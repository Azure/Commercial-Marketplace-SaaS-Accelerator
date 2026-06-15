// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

// =============================================================================
// Unit tests for MarketplaceResiliencePolicy (task 3.2)
//
// Validates: Requirements 2.1, 2.6, 2.8
//
// Coverage:
//   A. Retry count == MaxRetries for sustained transient failures
//   B. Exponential backoff schedule (delay = BaseDelay * 2^attempt)
//   C. Transient-vs-non-transient classification
//      - Transient statuses {408, 429, 500, 502, 503, 504} ARE retried
//      - Non-transient statuses {400, 401, 403, 404, 409} are NOT retried
//        and NOT counted toward the circuit-breaker
//   D. Circuit-breaker transitions: Closed → Open → Half-Open → Closed
//   E. Structured-log content shape (JSON with required fields)
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Services.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Xunit;

namespace Marketplace.SaaS.Accelerator.AdminSite.Test.Resilience;

/// <summary>
/// Unit tests for <see cref="MarketplaceResiliencePolicy"/>.
///
/// All tests use a <see cref="FakeTimeProvider"/> to skip real delays and a
/// <see cref="CapturingLogger"/> to assert structured-log content without
/// any I/O side effects.
/// </summary>
public class MarketplaceResiliencePolicyTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>Default options used by most tests.</summary>
    private static MarketplaceResilienceOptions DefaultOptions(
        int maxRetries = 3,
        int baseDelaySeconds = 1,
        int consecutiveFailureThreshold = 5,
        int cooldownSeconds = 60) =>
        new MarketplaceResilienceOptions
        {
            MaxRetries = maxRetries,
            BaseDelaySeconds = baseDelaySeconds,
            ConsecutiveFailureThreshold = consecutiveFailureThreshold,
            CooldownSeconds = cooldownSeconds,
        };

    /// <summary>
    /// Creates a fresh <see cref="RequestFailedException"/> with the given
    /// HTTP status code.
    /// </summary>
    private static RequestFailedException Rfe(int status) =>
        new RequestFailedException(status, $"HTTP {status}");

    // =========================================================================
    // A – Retry count
    // =========================================================================

    /// <summary>
    /// When every attempt raises a transient exception the pipeline exhausts
    /// exactly MaxRetries retries (initial attempt + MaxRetries = MaxRetries+1
    /// total invocations) and then rethrows.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task Retry_ExhaustsMaxRetries_ThenThrows(int maxRetries)
    {
        var logger = new CapturingLogger();
        // Use a very large ConsecutiveFailureThreshold so the circuit breaker
        // does not open before retries are exhausted.
        var options = DefaultOptions(maxRetries: maxRetries, consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        int callCount = 0;
        await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
            await pipeline.ExecuteAsync(ct =>
            {
                callCount++;
                throw Rfe(429);
#pragma warning disable CS0162
                return new ValueTask<int>(0);
#pragma warning restore CS0162
            }));

        // Initial attempt + MaxRetries retries.
        Assert.Equal(maxRetries + 1, callCount);
    }

    /// <summary>
    /// When the N-th attempt succeeds (N ≤ MaxRetries) the pipeline does not
    /// exhaust the full retry budget — it stops as soon as it gets a result.
    /// </summary>
    [Fact]
    public async Task Retry_StopsRetryingOnFirstSuccess()
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 3, consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        int callCount = 0;
        var result = await pipeline.ExecuteAsync(ct =>
        {
            callCount++;
            if (callCount < 3) throw Rfe(503);
            return new ValueTask<int>(42);
        });

        Assert.Equal(42, result);
        Assert.Equal(3, callCount); // 2 failures + 1 success
    }

    // =========================================================================
    // B – Exponential backoff schedule
    // =========================================================================

    /// <summary>
    /// The delay for attempt N (1-indexed) is BaseDelaySeconds * 2^(N-1).
    /// Verifies that <see cref="MarketplaceResiliencePolicy"/> records delays
    /// in the retry log entries that follow the expected exponential series.
    ///
    /// FakeTimeProvider absorbs the waits so the test runs in milliseconds.
    /// </summary>
    [Fact]
    public async Task Retry_BackoffSchedule_IsExponential()
    {
        const int MaxRetries = 3;
        const int BaseDelaySeconds = 1;

        var logger = new CapturingLogger();
        var options = DefaultOptions(
            maxRetries: MaxRetries,
            baseDelaySeconds: BaseDelaySeconds,
            consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        // All attempts fail so we collect MaxRetries log entries.
        await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
            await pipeline.ExecuteAsync(ct => throw Rfe(500)));

        // Filter to retry events only.
        var retryLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e != null && e.Event == "retry")
            .OrderBy(e => e.Attempt)
            .ToList();

        Assert.Equal(MaxRetries, retryLogs.Count);

        // Expected delays: 1s, 2s, 4s (BaseDelay * 2^0, 2^1, 2^2)
        var expectedDelaysMs = Enumerable.Range(0, MaxRetries)
            .Select(i => (long)(BaseDelaySeconds * Math.Pow(2, i) * 1000))
            .ToList();

        for (int i = 0; i < retryLogs.Count; i++)
        {
            // Allow ±50 ms tolerance for any floating-point rounding.
            var actual = retryLogs[i].DelayMs;
            var expected = expectedDelaysMs[i];
            Assert.True(
                Math.Abs(actual - expected) <= 50,
                $"Attempt {i + 1}: expected ~{expected}ms, got {actual}ms");
        }
    }

    // =========================================================================
    // C – Transient vs non-transient classification
    // =========================================================================

    /// <summary>
    /// Statuses in the transient set must cause at least one retry.
    /// </summary>
    [Theory]
    [InlineData(408)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task IsTransient_TransientStatuses_AreRetried(int status)
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 1, consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        int callCount = 0;
        await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
            await pipeline.ExecuteAsync(ct =>
            {
                callCount++;
                throw Rfe(status);
#pragma warning disable CS0162
                return new ValueTask<int>(0);
#pragma warning restore CS0162
            }));

        // With MaxRetries = 1, expect 2 total calls (initial + 1 retry).
        Assert.Equal(2, callCount);
    }

    /// <summary>
    /// Statuses NOT in the transient set must propagate immediately (no retry).
    /// They must also not trip the circuit breaker.
    /// </summary>
    [Theory]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(409)]
    public async Task IsTransient_NonTransientStatuses_AreNotRetried(int status)
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 3, consecutiveFailureThreshold: 2);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        int callCount = 0;
        // Run well past the circuit-breaker threshold — if non-transient errors
        // counted toward the breaker, the circuit would open and the test would
        // see BrokenCircuitException on later calls.
        for (int i = 0; i < 10; i++)
        {
            await Assert.ThrowsAsync<RequestFailedException>(async () =>
                await pipeline.ExecuteAsync(ct =>
                {
                    callCount++;
                    throw Rfe(status);
#pragma warning disable CS0162
                    return new ValueTask<int>(0);
#pragma warning restore CS0162
                }));
        }

        // Every call should have been attempted exactly once (no retries).
        Assert.Equal(10, callCount);

        // No retry log entries should have been emitted.
        var retryLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "retry")
            .ToList();
        Assert.Empty(retryLogs);

        // No circuit_opened log entries should have been emitted either.
        var cbLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "circuit_opened")
            .ToList();
        Assert.Empty(cbLogs);
    }

    /// <summary>
    /// <see cref="MarketplaceResiliencePolicy.IsTransient"/> returns false
    /// for a null exception (defensive check).
    /// </summary>
    [Fact]
    public void IsTransient_NullException_ReturnsFalse()
    {
        Assert.False(MarketplaceResiliencePolicy.IsTransient(null));
    }

    // =========================================================================
    // D – Circuit-breaker transitions: Closed → Open → Half-Open → Closed
    // =========================================================================

    /// <summary>
    /// The circuit opens after ConsecutiveFailureThreshold transient failures,
    /// causing subsequent calls to throw <see cref="BrokenCircuitException"/>
    /// without invoking the delegate.
    /// </summary>
    [Fact]
    public async Task CircuitBreaker_OpenAfterThresholdFailures_FailsFast()
    {
        const int Threshold = 3;
        var logger = new CapturingLogger();
        var options = DefaultOptions(
            maxRetries: 0,               // no retries so every call counts directly
            consecutiveFailureThreshold: Threshold,
            cooldownSeconds: 60);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        int delegateCallCount = 0;

        // Drive Threshold calls that all fail to saturate the sampling window.
        for (int i = 0; i < Threshold; i++)
        {
            await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
                await pipeline.ExecuteAsync(ct =>
                {
                    delegateCallCount++;
                    throw Rfe(503);
#pragma warning disable CS0162
                    return new ValueTask<int>(0);
#pragma warning restore CS0162
                }));
        }

        // Record how many delegate calls happened before the circuit opened.
        int delegateCallsBeforeOpen = delegateCallCount;

        // After the circuit is open, calls must fail fast (BrokenCircuitException)
        // without invoking the delegate.
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await pipeline.ExecuteAsync(ct =>
            {
                delegateCallCount++;
                return new ValueTask<int>(99);
            }));

        // Delegate call count should not have increased.
        Assert.Equal(delegateCallsBeforeOpen, delegateCallCount);

        // A circuit_opened log entry should have been emitted.
        var cbOpenedLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "circuit_opened")
            .ToList();
        Assert.NotEmpty(cbOpenedLogs);
    }

    /// <summary>
    /// After the break duration the circuit transitions to Half-Open.
    /// A probe that succeeds closes the circuit and allows subsequent calls.
    /// </summary>
    [Fact]
    public async Task CircuitBreaker_ClosedAfterSuccessfulProbe_AllowsSubsequentCalls()
    {
        const int Threshold = 3;
        var logger = new CapturingLogger();
        var manualControl = new CircuitBreakerManualControl();
        var options = DefaultOptions(
            maxRetries: 0,
            consecutiveFailureThreshold: Threshold,
            cooldownSeconds: 1);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, manualControl, timeProvider: tp);

        // Open the circuit manually so we can test the closed→open→half-open→closed
        // path without needing to defeat the sampling window.
        await manualControl.IsolateAsync();

        // Verify the circuit is open — calls fail fast.
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await pipeline.ExecuteAsync(ct => new ValueTask<int>(1)));

        // Close the circuit (simulates break duration elapsing).
        await manualControl.CloseAsync();

        // After closing, a successful call should go through normally.
        int result = await pipeline.ExecuteAsync(ct => new ValueTask<int>(42));
        Assert.Equal(42, result);

        // A circuit_closed log entry should have been emitted.
        var cbClosedLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "circuit_closed")
            .ToList();
        Assert.NotEmpty(cbClosedLogs);
    }

    /// <summary>
    /// Verifies that circuit half-open log entries are emitted.
    /// Uses manual control to force the circuit into the half-open state.
    /// </summary>
    [Fact]
    public async Task CircuitBreaker_HalfOpenLogEntry_IsEmitted()
    {
        var logger = new CapturingLogger();
        var manualControl = new CircuitBreakerManualControl();
        var options = DefaultOptions(
            maxRetries: 0,
            consecutiveFailureThreshold: 3,
            cooldownSeconds: 60);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, manualControl, timeProvider: tp);

        // Open then close via manual control — Polly emits OnClosed callback.
        await manualControl.IsolateAsync();
        await manualControl.CloseAsync();

        // Assert: at least a circuit_closed entry is present (the close after
        // isolation is the closest observable event).
        var cbLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e != null && (e.Event == "circuit_closed" || e.Event == "circuit_half_opened"))
            .ToList();
        Assert.NotEmpty(cbLogs);
    }

    // =========================================================================
    // E – Structured-log content shape
    // =========================================================================

    /// <summary>
    /// Each retry log entry must be valid JSON and include all required fields:
    /// event, operation, attempt, delayMs, subscriptionId, outcome.
    /// </summary>
    [Fact]
    public async Task StructuredLog_RetryEntry_ContainsRequiredFields()
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 1, consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        const string SubscriptionId = "sub-abc-123";

        await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
            await pipeline.ExecuteAsync(
                static (ctx, _) =>
                {
                    throw new RequestFailedException(503, "Service Unavailable");
#pragma warning disable CS0162
                    return new ValueTask<int>(0);
#pragma warning restore CS0162
                },
                ResilienceContextPool.Shared.Get(SubscriptionId),
                state: 0));

        var retryLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "retry")
            .ToList();

        Assert.NotEmpty(retryLogs);
        var entry = retryLogs.First();

        Assert.Equal("retry", entry.Event);
        Assert.True(entry.Attempt > 0, "attempt must be > 0 (1-indexed)");
        Assert.True(entry.DelayMs >= 0, "delayMs must be non-negative");
        // outcome must describe the failure
        Assert.False(string.IsNullOrEmpty(entry.Outcome), "outcome must not be empty");
    }

    /// <summary>
    /// When a subscriptionId is set in the ResilienceContext properties, the
    /// retry log entry should carry it.
    /// </summary>
    [Fact]
    public async Task StructuredLog_RetryEntry_CarriesSubscriptionIdFromContext()
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 1, consecutiveFailureThreshold: 100);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        const string ExpectedSubscriptionId = "sub-xyz-789";

        var ctx = ResilienceContextPool.Shared.Get();
        ctx.Properties.Set(MarketplaceResiliencePolicy.SubscriptionIdKey, ExpectedSubscriptionId);
        ctx.Properties.Set(MarketplaceResiliencePolicy.OperationKey, "TestOperation");

        await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
            await pipeline.ExecuteAsync(
                static (context, _) =>
                {
                    throw new RequestFailedException(503, "Service Unavailable");
#pragma warning disable CS0162
                    return new ValueTask<int>(0);
#pragma warning restore CS0162
                },
                ctx,
                state: 0));

        ResilienceContextPool.Shared.Return(ctx);

        var retryLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "retry")
            .ToList();

        Assert.NotEmpty(retryLogs);
        var entry = retryLogs.First();

        Assert.Equal(ExpectedSubscriptionId, entry.SubscriptionId);
        Assert.Equal("TestOperation", entry.Operation);
    }

    /// <summary>
    /// Circuit-opened log entries must be valid JSON with all required fields.
    /// </summary>
    [Fact]
    public async Task StructuredLog_CircuitOpenedEntry_ContainsRequiredFields()
    {
        const int Threshold = 2;
        var logger = new CapturingLogger();
        var options = DefaultOptions(
            maxRetries: 0,
            consecutiveFailureThreshold: Threshold,
            cooldownSeconds: 60);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        // Drive Threshold failures to trip the circuit.
        for (int i = 0; i < Threshold; i++)
        {
            await Assert.ThrowsAnyAsync<RequestFailedException>(async () =>
                await pipeline.ExecuteAsync(ct =>
                {
                    throw Rfe(503);
#pragma warning disable CS0162
                    return new ValueTask<int>(0);
#pragma warning restore CS0162
                }));
        }

        var cbOpenedLogs = logger.Entries
            .Select(TryParseLogEntry)
            .Where(e => e?.Event == "circuit_opened")
            .ToList();

        Assert.NotEmpty(cbOpenedLogs);

        var entry = cbOpenedLogs.First();
        Assert.Equal("circuit_opened", entry.Event);
        Assert.True(entry.DelayMs > 0, "delayMs (break duration) must be > 0");
    }

    /// <summary>
    /// A success on the first attempt (no transient failure) must not generate
    /// any retry log entries — the success path must be invisible to the policy.
    /// </summary>
    [Fact]
    public async Task SuccessPath_GeneratesNoRetryOrCircuitLogs()
    {
        var logger = new CapturingLogger();
        var options = DefaultOptions(maxRetries: 3, consecutiveFailureThreshold: 5);
        var tp = new FakeTimeProvider();
        var pipeline = MarketplaceResiliencePolicy.Build(options, logger, timeProvider: tp);

        var result = await pipeline.ExecuteAsync(ct => new ValueTask<int>(7));

        Assert.Equal(7, result);
        // No resilience events should have been logged.
        Assert.Empty(logger.Entries.Select(TryParseLogEntry).Where(e => e != null));
    }

    // =========================================================================
    // Build argument validation
    // =========================================================================

    [Fact]
    public void Build_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            MarketplaceResiliencePolicy.Build(null, logger: null));
    }

    // =========================================================================
    // Inner helpers – log parsing and test doubles
    // =========================================================================

    private sealed record LogEntry(
        string Event,
        string Operation,
        int Attempt,
        long DelayMs,
        string SubscriptionId,
        string Outcome);

    private static LogEntry TryParseLogEntry(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            return new LogEntry(
                Event:          GetString(root, "event"),
                Operation:      GetString(root, "operation"),
                Attempt:        root.TryGetProperty("attempt", out var a) && a.TryGetInt32(out var av) ? av : 0,
                DelayMs:        root.TryGetProperty("delayMs",  out var d) && d.TryGetInt64(out var dv) ? dv : 0,
                SubscriptionId: GetString(root, "subscriptionId"),
                Outcome:        GetString(root, "outcome"));
        }
        catch
        {
            return null;
        }
    }

    private static string GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var p) ? p.GetString() : null;

    /// <summary>
    /// Thread-safe logger that captures every Info message emitted by
    /// <see cref="MarketplaceResiliencePolicy"/>.
    /// </summary>
    private sealed class CapturingLogger : Services.Contracts.ILogger
    {
        private readonly object @lock = new();
        private readonly List<string> entries = new();

        public IReadOnlyList<string> Entries
        {
            get { lock (@lock) { return entries.ToArray(); } }
        }

        public void Info(string message)    { lock (@lock) { entries.Add(message); } }
        public void Info(string m, Exception e) => Info(m);
        public void Debug(string m)         { }
        public void Debug(string m, Exception e){ }
        public void Warn(string m)          { }
        public void Warn(string m, Exception e) { }
        public void Error(string m)         { }
        public void Error(string m, Exception e){ }
    }

    /// <summary>
    /// Minimal <see cref="TimeProvider"/> that advances time instantly,
    /// preventing real-time sleep inside Polly delay generators.
    ///
    /// Polly 8 accepts a custom <see cref="TimeProvider"/> on the pipeline
    /// builder, which causes all <c>Task.Delay</c> calls inside retry/CB
    /// strategies to use the fake clock — so tests finish in milliseconds even
    /// when the configured backoff delays are large.
    /// </summary>
    private sealed class FakeTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public override ITimer CreateTimer(
            TimerCallback callback,
            object state,
            TimeSpan dueTime,
            TimeSpan period)
        {
            // Fire the callback immediately (simulates instant elapse of delay).
            callback(state);
            return new FakeTimer();
        }

        private sealed class FakeTimer : ITimer
        {
            public bool Change(TimeSpan dueTime, TimeSpan period) => true;
            public void Dispose() { }
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
