// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Marketplace.SaaS.Accelerator.Services.Services.Resilience;

/// <summary>
/// Builds a composed Polly 8.x <see cref="ResiliencePipeline"/> that applies
/// exponential-backoff retry (inner) wrapped by a circuit breaker (outer) to
/// every Marketplace API call.
///
/// <para>
/// The predicate handles <see cref="RequestFailedException"/> whose HTTP status
/// is in the transient set {408, 429, 500, 502, 503, 504}.  Non-transient
/// statuses (400, 401, 403, 404, 409) are NOT retried and are NOT counted toward
/// the circuit-breaker failure ratio.
/// </para>
///
/// <para>
/// Every resilience event (retry, circuit opened/closed/half-opened) is emitted
/// as a single structured JSON log line via the supplied <see cref="ILogger"/>.
/// The log entry contains: <c>event</c>, <c>operation</c>, <c>attempt</c>,
/// <c>delayMs</c>, <c>subscriptionId</c>, <c>outcome</c>.
/// </para>
///
/// <para>
/// The caller may optionally supply a <see cref="CircuitBreakerManualControl"/>
/// (for test isolation / manual circuit management) and a <see cref="TimeProvider"/>
/// (to accelerate time in unit tests).
/// </para>
/// </summary>
public static class MarketplaceResiliencePolicy
{
    // --------------------------------------------------------------------------
    // Well-known context-property keys for structured-log extraction.
    // Callers (e.g. FulfillmentApiServiceWithPolicy) set these on the
    // ResilienceContext before calling ExecuteAsync.
    // --------------------------------------------------------------------------

    /// <summary>Property key for the calling subscription identifier.</summary>
    public static readonly ResiliencePropertyKey<string> SubscriptionIdKey =
        new ResiliencePropertyKey<string>("marketplace.subscriptionId");

    /// <summary>Property key for the calling operation name.</summary>
    public static readonly ResiliencePropertyKey<string> OperationKey =
        new ResiliencePropertyKey<string>("marketplace.operation");

    // --------------------------------------------------------------------------
    // HTTP status codes that represent transient / retryable Marketplace errors.
    // --------------------------------------------------------------------------
    private static readonly HashSet<int> TransientStatusCodes = new HashSet<int>
    {
        408, // Request Timeout
        429, // Too Many Requests
        500, // Internal Server Error
        502, // Bad Gateway
        503, // Service Unavailable
        504, // Gateway Timeout
    };

    // --------------------------------------------------------------------------
    // Public factory
    // --------------------------------------------------------------------------

    /// <summary>
    /// Builds and returns a <see cref="ResiliencePipeline"/> composed of:
    /// <list type="bullet">
    ///   <item>Outer circuit breaker – trips when the failure ratio inside the
    ///   sampling window exceeds the threshold. Fail-fast for the cooldown
    ///   period, then half-open probe before re-closing.</item>
    ///   <item>Inner retry – exponential backoff up to <see cref="MarketplaceResilienceOptions.MaxRetries"/>.</item>
    /// </list>
    /// Only <see cref="RequestFailedException"/> with a transient status code is
    /// handled.  Non-transient exceptions propagate immediately and are not
    /// counted toward the breaker.
    /// </summary>
    /// <param name="options">
    ///   Configuration knobs; all resilience parameters are read from here.
    /// </param>
    /// <param name="logger">
    ///   Receives a structured JSON log line for each resilience event.  May be
    ///   <c>null</c> (logging is suppressed).
    /// </param>
    /// <param name="manualControl">
    ///   Optional <see cref="CircuitBreakerManualControl"/> for programmatic
    ///   circuit management (e.g. test isolation). When <c>null</c> the circuit
    ///   is governed entirely by failure statistics.
    /// </param>
    /// <param name="timeProvider">
    ///   Optional <see cref="TimeProvider"/> injected into the pipeline. Pass a
    ///   fake/manual implementation in unit tests to skip real-time waits.
    ///   When <c>null</c> <see cref="TimeProvider.System"/> is used.
    /// </param>
    public static ResiliencePipeline Build(
        MarketplaceResilienceOptions options,
        ILogger logger,
        CircuitBreakerManualControl manualControl = null,
        TimeProvider timeProvider = null)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        // ----------------------------------------------------------------
        // Circuit-breaker strategy options (outer strategy)
        //
        // Design choice: FailureRatio = 0.99 approximates "consecutive
        // failures" in Polly 8.x (which uses a sampling window instead of
        // a strict consecutive counter).
        //   - MinimumThroughput = ConsecutiveFailureThreshold
        //   - FailureRatio      = 0.99 (all but 1% of calls must fail)
        //   - SamplingDuration  = CooldownSeconds * 2
        //
        // Practically: when all N calls in the sampling window fail, the
        // circuit opens.  One successful call within the window lowers the
        // failure ratio below the threshold and prevents opening — exactly
        // the desired behaviour for "consecutive" failures.
        // ----------------------------------------------------------------
        var cbOptions = new CircuitBreakerStrategyOptions
        {
            ShouldHandle = new PredicateBuilder()
                .Handle<RequestFailedException>(IsTransient),

            FailureRatio = 0.99,
            MinimumThroughput = options.ConsecutiveFailureThreshold,
            SamplingDuration = TimeSpan.FromSeconds(options.CooldownSeconds * 2),
            BreakDuration = TimeSpan.FromSeconds(options.CooldownSeconds),

            ManualControl = manualControl,

            OnOpened = args =>
            {
                args.Context.Properties.TryGetValue(SubscriptionIdKey, out var subscriptionId);
                args.Context.Properties.TryGetValue(OperationKey, out var operation);
                EmitLog(logger, "circuit_opened", operation ?? args.Context.OperationKey, attempt: 0,
                    delayMs: (long)args.BreakDuration.TotalMilliseconds,
                    subscriptionId: subscriptionId,
                    outcome: args.Outcome.Exception?.Message ?? "failure threshold exceeded");
                return new System.Threading.Tasks.ValueTask();
            },

            OnClosed = args =>
            {
                args.Context.Properties.TryGetValue(SubscriptionIdKey, out var subscriptionId);
                args.Context.Properties.TryGetValue(OperationKey, out var operation);
                EmitLog(logger, "circuit_closed", operation ?? args.Context.OperationKey, attempt: 0,
                    delayMs: 0,
                    subscriptionId: subscriptionId,
                    outcome: "reset");
                return new System.Threading.Tasks.ValueTask();
            },

            OnHalfOpened = args =>
            {
                args.Context.Properties.TryGetValue(SubscriptionIdKey, out var subscriptionId);
                args.Context.Properties.TryGetValue(OperationKey, out var operation);
                EmitLog(logger, "circuit_half_opened", operation ?? args.Context.OperationKey, attempt: 0,
                    delayMs: 0,
                    subscriptionId: subscriptionId,
                    outcome: "probing");
                return new System.Threading.Tasks.ValueTask();
            },
        };

        // ----------------------------------------------------------------
        // Retry strategy options (inner strategy)
        // delay = BaseDelaySeconds * 2^AttemptNumber  (0-indexed)
        //       = BaseDelaySeconds * 2^(attempt-1)    (1-indexed, as in spec)
        // ----------------------------------------------------------------
        var retryOptions = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder()
                .Handle<RequestFailedException>(IsTransient),

            MaxRetryAttempts = options.MaxRetries,

            DelayGenerator = args =>
            {
                var delay = TimeSpan.FromSeconds(
                    options.BaseDelaySeconds * Math.Pow(2, args.AttemptNumber));
                return new System.Threading.Tasks.ValueTask<TimeSpan?>(delay);
            },

            OnRetry = args =>
            {
                args.Context.Properties.TryGetValue(SubscriptionIdKey, out var subscriptionId);
                args.Context.Properties.TryGetValue(OperationKey, out var operation);
                EmitLog(logger, "retry", operation ?? args.Context.OperationKey,
                    attempt: args.AttemptNumber + 1,                     // 1-indexed
                    delayMs: (long)args.RetryDelay.TotalMilliseconds,
                    subscriptionId: subscriptionId,
                    outcome: args.Outcome.Exception?.Message ?? "unknown error");
                return new System.Threading.Tasks.ValueTask();
            },
        };

        // ----------------------------------------------------------------
        // Assemble the pipeline: CB (outer) → Retry (inner) → user action.
        // Circuit breaker sees the final outcome after retries; it opens
        // when the fully-retried operation still fails.
        //
        // Polly 8.x requires MaxRetryAttempts >= 1.  When the caller
        // configures MaxRetries = 0 (no-retry mode, used for circuit-breaker
        // isolation in tests) we simply omit the retry layer entirely.
        // ----------------------------------------------------------------
        var builder = new ResiliencePipelineBuilder();
        builder.AddCircuitBreaker(cbOptions);

        if (options.MaxRetries > 0)
            builder.AddRetry(retryOptions);

        if (timeProvider != null)
            builder.TimeProvider = timeProvider;

        return builder.Build();
    }

    // --------------------------------------------------------------------------
    // Helpers
    // --------------------------------------------------------------------------

    /// <summary>
    /// Returns <c>true</c> for HTTP status codes that indicate a transient
    /// Marketplace API failure — the predicate is shared by both the retry and
    /// circuit-breaker strategies so they handle exactly the same exception set.
    /// </summary>
    public static bool IsTransient(RequestFailedException ex) =>
        ex != null && TransientStatusCodes.Contains(ex.Status);

    /// <summary>
    /// Emits a single structured JSON log line via the supplied logger.
    /// Silently swallows serialization failures so the resilience path is never
    /// disrupted by logging errors.
    /// </summary>
    private static void EmitLog(
        ILogger logger,
        string eventName,
        string operation,
        int attempt,
        long delayMs,
        string subscriptionId,
        string outcome)
    {
        if (logger == null)
            return;

        try
        {
            var entry = JsonSerializer.Serialize(new
            {
                @event = eventName,
                operation = operation ?? "unknown",
                attempt,
                delayMs,
                subscriptionId = subscriptionId ?? string.Empty,
                outcome = outcome ?? string.Empty,
            });
            logger.Info(entry);
        }
        catch
        {
            // Swallow — logging must not disrupt the resilience path.
        }
    }
}
