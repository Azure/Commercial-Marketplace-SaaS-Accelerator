// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Marketplace.SaaS.Accelerator.AdminSite.Controllers;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Doubles;
using Marketplace.SaaS.Accelerator.AdminSite.Test.Fixtures;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Services.Resilience;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ContractsILogger = Marketplace.SaaS.Accelerator.Services.Contracts.ILogger;
using ExtensionsILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Logging.Abstractions;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.BugCondition;

/// <summary>
/// Builder that wires a real <see cref="HomeController"/> against in-memory
/// repositories and the fakes from tasks 1.2 and 1.3.
///
/// HomeController's constructor signature (the auth seam, the data access
/// seam, the Marketplace SaaS seam, and the SDK config seam) is reproduced
/// exactly — the test exercises the same code path the production AdminSite
/// uses, with only the network/database providers swapped:
///
///   - <c>IMarketplaceSaaSClient</c> is replaced by the
///     <see cref="FakeMarketplaceSaaSClientBuilder"/> output, wrapped in a
///     real <see cref="FulfillmentApiService"/> (per task 1.4 deliverable 1:
///     "no stubbing of FulfillmentApiService itself").
///   - <see cref="SaasKitContext"/> is replaced by the InMemory-provider
///     instance from <see cref="InMemorySaasKitContextFactory"/>.
///   - The auth seam — <c>this.User.Identity.IsAuthenticated</c> and
///     <c>this.CurrentUserEmailAddress</c> — is satisfied by setting a
///     <see cref="ControllerContext"/> with a <see cref="DefaultHttpContext"/>
///     whose <see cref="ClaimsPrincipal"/> carries the seeded user's email
///     in the <c>preferred_username</c> claim (matches
///     <c>ClaimConstants.CLAIM_EMAILADDRESS</c>).
///   - Other dependencies that the FetchAllSubscriptions / Subscriptions
///     code paths do not touch are filled in with permissive Moq stubs.
/// </summary>
internal static class HomeControllerHarness
{
    /// <summary>Email used for the synthetic admin user seeded into the in-memory context.</summary>
    public const string AdminEmail = "admin@bugcondition.test";

    /// <summary>The "preferred_username" claim type the AdminSite uses for the user email.</summary>
    public const string EmailClaimType = "preferred_username";

    /// <summary>
    /// Materialise a controller plus the observation hooks the property test
    /// asserts against.
    /// </summary>
    public static Built Build(
        SubscriptionCorpus corpus,
        FakeMarketplaceSaaSClient fakeClient)
    {
        if (corpus is null) throw new ArgumentNullException(nameof(corpus));
        if (fakeClient is null) throw new ArgumentNullException(nameof(fakeClient));

        // Seed a synthetic admin user into the corpus so
        // GetUserIdFromEmailAddress(AdminEmail) succeeds.
        // Assign a UserId that won't conflict with the generated users
        // (generated users have sequential IDs starting at 1).
        var maxExistingUserId = corpus.Users.Count > 0
            ? corpus.Users.Max(u => u.UserId)
            : 0;
        var seededCorpus = new SubscriptionCorpus
        {
            Offers = corpus.Offers,
            Plans = corpus.Plans,
            Users = corpus.Users.Concat(new[]
            {
                new Users { UserId = maxExistingUserId + 1, EmailAddress = AdminEmail, FullName = "Admin", CreatedDate = DateTime.UtcNow },
            }).ToList(),
            Subscriptions = corpus.Subscriptions,
            AuditLogs = corpus.AuditLogs,
        };

        var ctx = InMemorySaasKitContextFactory.CreateAndSeed(seededCorpus);

        var subscriptionsRepo = new SubscriptionsRepository(ctx);
        var appConfigRepo = new ApplicationConfigRepository(ctx);
        var plansRepo = new PlansRepository(ctx, appConfigRepo);
        var usersRepo = new UsersRepository(ctx);
        var subscriptionLogsRepo = new SubscriptionLogRepository(ctx);
        var offersRepo = new OffersRepository(ctx);
        var applicationLogRepo = new ApplicationLogRepository(ctx);

        var sdkConfig = new SaaSApiClientConfiguration
        {
            FulFillmentAPIBaseURL = "https://localhost/fake-marketplace",
            FulFillmentAPIVersion = "2018-08-31",
        };

        // Construct the production FulfillmentApiService over the fake client,
        // then wrap it with the Polly resilience decorator so retry/circuit-
        // breaker logic fires during the property test (task 3.9).
        var capturedLogs = new CapturedLoggerSink();
        var fulfillmentLogger = new SinkBackedContractsILogger(capturedLogs);
        var innerFulfillmentApiService = new FulfillmentApiService(
            fakeClient.Client,
            sdkConfig,
            fulfillmentLogger);

        // Build the resilience pipeline with no real delays in tests by using
        // options with BaseDelaySeconds=0.  The circuit-breaker thresholds match
        // the defaults (ConsecutiveFailureThreshold=5, MaxRetries=3).
        var resilienceOptionsValue = new MarketplaceResilienceOptions
        {
            MaxRetries = 3,
            BaseDelaySeconds = 0,         // no real waits in tests
            ConsecutiveFailureThreshold = 5,
            CooldownSeconds = 1,          // short cooldown so circuit re-opens fast
            MaxConcurrentPlanFetches = FetchPipelineBugConditionPropertyTests.MaxConcurrentPlanFetches,
        };
        var capturedResilienceLogger = new SinkBackedContractsILogger(capturedLogs);
        var resiliencePipeline = MarketplaceResiliencePolicy.Build(resilienceOptionsValue, capturedResilienceLogger);
        var fulfillmentApiService = new FulfillmentApiServiceWithPolicy(innerFulfillmentApiService, resiliencePipeline);

        var loggerFactory = new CapturedLoggerFactory(capturedLogs);

        // Mocks for dependencies the code paths under test don't touch.
        // KnownUserAttribute is registered as a ServiceFilter on HomeController,
        // but we invoke action methods directly so the filter never fires.
        var billingApiService = new Mock<IMeteredBillingApiService>(MockBehavior.Loose).Object;
        var subscriptionUsageLogsRepository = new Mock<ISubscriptionUsageLogsRepository>(MockBehavior.Loose).Object;
        var dimensionsRepository = new Mock<IMeteredDimensionsRepository>(MockBehavior.Loose).Object;
        var emailTemplateRepository = new Mock<IEmailTemplateRepository>(MockBehavior.Loose).Object;
        var planEventsMappingRepository = new Mock<IPlanEventsMappingRepository>(MockBehavior.Loose).Object;
        var eventsRepository = new Mock<IEventsRepository>(MockBehavior.Loose).Object;
        var emailService = new Mock<IEmailService>(MockBehavior.Loose).Object;
        var offersAttributeRepository = new Mock<IOfferAttributesRepository>(MockBehavior.Loose).Object;

        var appVersionService = new Mock<IAppVersionService>();
        appVersionService.SetupGet(s => s.Version).Returns("test-1.0.0");

        var gitReleases = new Mock<ISAGitReleasesService>();
        gitReleases.Setup(g => g.GetLatestReleaseFromGitHub()).Returns(string.Empty);

        // SaaSClientLogger<HomeController> is a concrete class; we use the real
        // one (it constructs its own console logger). The structured-log
        // assertion in the property test reads the captured logs from the
        // sink wired through fulfillmentLogger and from the
        // ApplicationLogRepository (where the controller's catch block writes
        // unstructured strings on UNFIXED code).
        var clientLogger = new SaaSClientLogger<HomeController>();

        var resilienceOptionsInstance = Microsoft.Extensions.Options.Options.Create(resilienceOptionsValue);

        var pipeline = new SubscriptionFetchPipeline(
            fulfillApiService: fulfillmentApiService,
            subscriptionsRepository: subscriptionsRepo,
            plansRepository: plansRepo,
            offersRepository: offersRepo,
            usersRepository: usersRepo,
            subscriptionLogRepository: subscriptionLogsRepo,
            options: resilienceOptionsInstance,
            logger: new SinkBackedTypedLogger<SubscriptionFetchPipeline>(capturedLogs));

        var controller = new HomeController(
            usersRepository: usersRepo,
            billingApiService: billingApiService,
            subscriptionRepo: subscriptionsRepo,
            planRepository: plansRepo,
            subscriptionUsageLogsRepository: subscriptionUsageLogsRepository,
            dimensionsRepository: dimensionsRepository,
            subscriptionLogsRepo: subscriptionLogsRepo,
            applicationConfigRepository: appConfigRepo,
            userRepository: usersRepo,
            fulfillApiService: fulfillmentApiService,
            applicationLogRepository: applicationLogRepo,
            emailTemplateRepository: emailTemplateRepository,
            planEventsMappingRepository: planEventsMappingRepository,
            eventsRepository: eventsRepository,
            saaSApiClientConfiguration: sdkConfig,
            loggerFactory: loggerFactory,
            emailService: emailService,
            offersRepository: offersRepo,
            offersAttributeRepository: offersAttributeRepository,
            appVersionService: appVersionService.Object,
            sAGitReleasesService: gitReleases.Object,
            resilienceOptions: resilienceOptionsInstance,
            subscriptionFetchPipeline: pipeline,
            logger: clientLogger);

        // Wire ControllerContext with an authenticated principal carrying the
        // seeded admin's email in the preferred_username claim.
        var identity = new ClaimsIdentity(
            new[] { new Claim(EmailClaimType, AdminEmail) },
            authenticationType: "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };

        // Wire TempData so actions that set/read TempData do not throw
        // InvalidOperationException / NullReferenceException.
        controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            httpContext,
            Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

        return new Built(controller, ctx, applicationLogRepo, capturedLogs);
    }

    /// <summary>
    /// Build a new controller that shares an existing <see cref="SaasKitContext"/>
    /// with a different <see cref="FakeMarketplaceSaaSClient"/>. This is used by
    /// the audit-log snapshot test (task 2.2) to run a second FetchAllSubscriptions
    /// call on the same in-memory database so the second call can detect changes
    /// written by the first call.
    /// </summary>
    public static Built BuildWithExistingContext(
        SaasKitContext ctx,
        FakeMarketplaceSaaSClient fakeClient)
    {
        if (ctx is null) throw new ArgumentNullException(nameof(ctx));
        if (fakeClient is null) throw new ArgumentNullException(nameof(fakeClient));

        var subscriptionsRepo = new SubscriptionsRepository(ctx);
        var appConfigRepo = new ApplicationConfigRepository(ctx);
        var plansRepo = new PlansRepository(ctx, appConfigRepo);
        var usersRepo = new UsersRepository(ctx);
        var subscriptionLogsRepo = new SubscriptionLogRepository(ctx);
        var offersRepo = new OffersRepository(ctx);
        var applicationLogRepo = new ApplicationLogRepository(ctx);

        var sdkConfig = new SaaSApiClientConfiguration
        {
            FulFillmentAPIBaseURL = "https://localhost/fake-marketplace",
            FulFillmentAPIVersion = "2018-08-31",
        };

        var capturedLogs = new CapturedLoggerSink();
        var fulfillmentLogger = new SinkBackedContractsILogger(capturedLogs);
        var innerFulfillmentApiService = new FulfillmentApiService(
            fakeClient.Client,
            sdkConfig,
            fulfillmentLogger);

        // Wrap with Polly resilience policy (same config as Build()).
        var resilienceOptionsValue2 = new MarketplaceResilienceOptions
        {
            MaxRetries = 3,
            BaseDelaySeconds = 0,
            ConsecutiveFailureThreshold = 5,
            CooldownSeconds = 1,
            MaxConcurrentPlanFetches = FetchPipelineBugConditionPropertyTests.MaxConcurrentPlanFetches,
        };
        var resiliencePipeline2 = MarketplaceResiliencePolicy.Build(
            resilienceOptionsValue2,
            new SinkBackedContractsILogger(capturedLogs));
        var fulfillmentApiService = new FulfillmentApiServiceWithPolicy(innerFulfillmentApiService, resiliencePipeline2);

        var loggerFactory = new CapturedLoggerFactory(capturedLogs);

        var billingApiService = new Mock<IMeteredBillingApiService>(MockBehavior.Loose).Object;
        var subscriptionUsageLogsRepository = new Mock<ISubscriptionUsageLogsRepository>(MockBehavior.Loose).Object;
        var dimensionsRepository = new Mock<IMeteredDimensionsRepository>(MockBehavior.Loose).Object;
        var emailTemplateRepository = new Mock<IEmailTemplateRepository>(MockBehavior.Loose).Object;
        var planEventsMappingRepository = new Mock<IPlanEventsMappingRepository>(MockBehavior.Loose).Object;
        var eventsRepository = new Mock<IEventsRepository>(MockBehavior.Loose).Object;
        var emailService = new Mock<IEmailService>(MockBehavior.Loose).Object;
        var offersAttributeRepository = new Mock<IOfferAttributesRepository>(MockBehavior.Loose).Object;

        var appVersionService = new Mock<IAppVersionService>();
        appVersionService.SetupGet(s => s.Version).Returns("test-1.0.0");

        var gitReleases = new Mock<ISAGitReleasesService>();
        gitReleases.Setup(g => g.GetLatestReleaseFromGitHub()).Returns(string.Empty);

        var clientLogger = new SaaSClientLogger<HomeController>();

        var resilienceOptionsInstance2 = Microsoft.Extensions.Options.Options.Create(resilienceOptionsValue2);

        var pipeline2 = new SubscriptionFetchPipeline(
            fulfillApiService: fulfillmentApiService,
            subscriptionsRepository: subscriptionsRepo,
            plansRepository: plansRepo,
            offersRepository: offersRepo,
            usersRepository: usersRepo,
            subscriptionLogRepository: subscriptionLogsRepo,
            options: resilienceOptionsInstance2,
            logger: new SinkBackedTypedLogger<SubscriptionFetchPipeline>(capturedLogs));

        var controller = new HomeController(
            usersRepository: usersRepo,
            billingApiService: billingApiService,
            subscriptionRepo: subscriptionsRepo,
            planRepository: plansRepo,
            subscriptionUsageLogsRepository: subscriptionUsageLogsRepository,
            dimensionsRepository: dimensionsRepository,
            subscriptionLogsRepo: subscriptionLogsRepo,
            applicationConfigRepository: appConfigRepo,
            userRepository: usersRepo,
            fulfillApiService: fulfillmentApiService,
            applicationLogRepository: applicationLogRepo,
            emailTemplateRepository: emailTemplateRepository,
            planEventsMappingRepository: planEventsMappingRepository,
            eventsRepository: eventsRepository,
            saaSApiClientConfiguration: sdkConfig,
            loggerFactory: loggerFactory,
            emailService: emailService,
            offersRepository: offersRepo,
            offersAttributeRepository: offersAttributeRepository,
            appVersionService: appVersionService.Object,
            sAGitReleasesService: gitReleases.Object,
            resilienceOptions: resilienceOptionsInstance2,
            subscriptionFetchPipeline: pipeline2,
            logger: clientLogger);

        var identity = new ClaimsIdentity(
            new[] { new Claim(EmailClaimType, AdminEmail) },
            authenticationType: "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var httpContext2 = new DefaultHttpContext { User = principal };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext2,
        };

        // Wire TempData so actions that set/read TempData do not throw.
        controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            httpContext2,
            Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

        return new Built(controller, ctx, applicationLogRepo, capturedLogs);
    }

    /// <summary>Bundle of objects the property test needs after building the harness.</summary>
    public sealed record Built(
        HomeController Controller,
        SaasKitContext Context,
        IApplicationLogRepository ApplicationLogs,
        CapturedLoggerSink CapturedLogs);

    /// <summary>
    /// Thread-safe sink that records every log message emitted through the
    /// captured logger factory, plus the unstructured strings the unfixed
    /// FulfillmentApiService writes via <c>SaaSClientLogger</c>'s
    /// <see cref="ContractsILogger"/> surface (info / warn / error).
    /// </summary>
    public sealed class CapturedLoggerSink
    {
        private readonly object @lock = new();
        private readonly List<string> entries = new();

        public IReadOnlyList<string> Entries
        {
            get { lock (@lock) { return entries.ToArray(); } }
        }

        public void Add(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            lock (@lock) { entries.Add(message); }
        }

        public bool AnyContains(string substring) =>
            Entries.Any(e => e.IndexOf(substring, StringComparison.Ordinal) >= 0);
    }

    /// <summary>
    /// <see cref="ILoggerFactory"/> that hands out
    /// <see cref="ExtensionsILogger"/> instances which append every formatted
    /// message to a shared <see cref="CapturedLoggerSink"/>.
    /// </summary>
    private sealed class CapturedLoggerFactory : ILoggerFactory
    {
        private readonly CapturedLoggerSink sink;

        public CapturedLoggerFactory(CapturedLoggerSink sink) => this.sink = sink;

        public void AddProvider(ILoggerProvider provider) { /* no-op */ }

        public ExtensionsILogger CreateLogger(string categoryName) => new Sink(sink);

        public void Dispose() { /* no-op */ }

        private sealed class Sink : ExtensionsILogger
        {
            private readonly CapturedLoggerSink sink;

            public Sink(CapturedLoggerSink sink) => this.sink = sink;

            public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                if (formatter is null) return;
                var message = formatter(state, exception);
                sink.Add(message);
            }

            private sealed class NullScope : IDisposable
            {
                public static readonly NullScope Instance = new();
                public void Dispose() { }
            }
        }
    }

    /// <summary>
    /// Adapter so a <see cref="ContractsILogger"/> surface (used by
    /// <see cref="FulfillmentApiService"/>) routes every emitted message
    /// into the sink.
    /// </summary>
    private sealed class SinkBackedContractsILogger : ContractsILogger
    {
        private readonly CapturedLoggerSink sink;

        public SinkBackedContractsILogger(CapturedLoggerSink sink) => this.sink = sink;

        public void Debug(string message) => sink.Add(message);
        public void Debug(string message, Exception ex) => sink.Add(message);
        public void Error(string message) => sink.Add(message);
        public void Error(string message, Exception ex) => sink.Add(message);
        public void Info(string message) => sink.Add(message);
        public void Info(string message, Exception ex) => sink.Add(message);
        public void Warn(string message) => sink.Add(message);
        public void Warn(string message, Exception ex) => sink.Add(message);
    }

    /// <summary>
    /// Typed <see cref="ILogger{T}"/> backed by <see cref="CapturedLoggerSink"/>
    /// so log output from services that require <c>ILogger&lt;T&gt;</c>
    /// (e.g. <see cref="SubscriptionFetchPipeline"/>) is captured for assertions.
    /// </summary>
    internal sealed class SinkBackedTypedLogger<T> : ExtensionsILogger, ILogger<T>
    {
        private readonly CapturedLoggerSink sink;

        public SinkBackedTypedLogger(CapturedLoggerSink sink) => this.sink = sink;

        public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter is null) return;
            var message = formatter(state, exception);
            sink.Add(message);
        }

        private sealed class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new();
            public void Dispose() { }
        }
    }
}
