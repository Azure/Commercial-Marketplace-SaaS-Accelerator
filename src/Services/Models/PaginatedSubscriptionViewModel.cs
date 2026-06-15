// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// View model for the paginated Subscriptions list page.
/// Extends <see cref="SubscriptionViewModel"/> with pagination metadata,
/// empty-state flags, and background sync configuration so the view can
/// render pagination controls and guidance text without additional queries.
/// </summary>
public class PaginatedSubscriptionViewModel : SubscriptionViewModel
{
    /// <summary>
    /// Gets or sets the total number of subscriptions across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the 1-based page index of the current page (clamped to >= 1).
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page (clamped to >= 1).
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the subscriptions table is empty
    /// (i.e., <see cref="TotalCount"/> == 0). When true, the view renders the
    /// enriched empty-state guidance panel instead of the generic empty table.
    /// </summary>
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the background lazy-loader
    /// hosted service is enabled. Surfaces <c>MarketplaceResilienceOptions.BackgroundSyncEnabled</c>
    /// so the empty-state panel can conditionally reference the sync interval.
    /// </summary>
    public bool BackgroundSyncEnabled { get; set; }

    /// <summary>
    /// Gets or sets the interval in seconds between background sync ticks.
    /// Surfaces <c>MarketplaceResilienceOptions.BackgroundSyncIntervalSeconds</c>
    /// so the empty-state panel can display a human-readable wait time.
    /// </summary>
    public int BackgroundSyncIntervalSeconds { get; set; }
}
