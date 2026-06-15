// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

/// <summary>
/// Represents a single page of results from a paginated query, together with
/// the metadata needed to render pagination controls.
/// </summary>
/// <typeparam name="T">The entity type for items in the page.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Gets or sets the items in this page.</summary>
    public IReadOnlyList<T> Items { get; set; }

    /// <summary>Gets or sets the total number of items across all pages.</summary>
    public int TotalCount { get; set; }

    /// <summary>Gets or sets the 1-based page index (already clamped to >= 1).</summary>
    public int PageIndex { get; set; }

    /// <summary>Gets or sets the page size (already clamped to >= 1).</summary>
    public int PageSize { get; set; }
}
