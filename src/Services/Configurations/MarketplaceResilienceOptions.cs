// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace Marketplace.SaaS.Accelerator.Services.Configurations;

/// <summary>
/// Configuration options for Marketplace API resilience and background sync behaviour.
/// Bind from the "MarketplaceResilience" section of appsettings.json.
/// </summary>
public class MarketplaceResilienceOptions
{
    /// <summary>
    /// Maximum number of retry attempts for transient Marketplace API failures.
    /// Default: 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Base delay in seconds for the first retry. Subsequent retries use exponential backoff
    /// (BaseDelaySeconds * 2^(attempt-1)).
    /// Default: 1 second.
    /// </summary>
    public int BaseDelaySeconds { get; set; } = 1;

    /// <summary>
    /// Number of consecutive failures that will trip the circuit breaker.
    /// Default: 5.
    /// </summary>
    public int ConsecutiveFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Duration in seconds that the circuit breaker stays open (cooling down) before
    /// transitioning to half-open and probing the upstream service.
    /// Default: 60 seconds.
    /// </summary>
    public int CooldownSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum number of concurrent <c>GetAllPlansForSubscriptionAsync</c> calls during a
    /// single FetchAllSubscriptions run.  Enforced via <c>SemaphoreSlim</c>.
    /// Default: 5. Raise this for high-volume publishers to take advantage of the
    /// Marketplace API quota (400 req/min); lower it if you encounter 429s under load.
    /// </summary>
    public int MaxConcurrentPlanFetches { get; set; } = 5;

    /// <summary>
    /// Interval in seconds between background lazy-loader sync ticks.
    /// Default: 300 seconds (5 minutes).
    /// </summary>
    public int BackgroundSyncIntervalSeconds { get; set; } = 300;

    /// <summary>
    /// Whether the background lazy-loader hosted service is enabled.
    /// Default: true.
    /// </summary>
    public bool BackgroundSyncEnabled { get; set; } = true;

    /// <summary>
    /// Number of subscriptions returned per page on the Subscriptions list view.
    /// Default: 100.
    /// </summary>
    public int PageSize { get; set; } = 100;

    /// <summary>
    /// EF Core command timeout in seconds for database queries.
    /// Default: 30 seconds.
    /// </summary>
    public int DatabaseQueryTimeoutSeconds { get; set; } = 30;
}
