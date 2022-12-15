// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// PlanDetail Result Extension.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.PlanDetailResult" />
public class PlanDetailResultExtension : PlanDetailResult
{
    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    public Guid OfferId { get; set; }

    /// <summary>
    /// Gets or sets the plan unique identifier.
    /// </summary>
    /// <value>
    /// The plan unique identifier.
    /// </value>
    public Guid PlanGUID { get; set; }

    /// <summary>
    /// Gets or sets the plan per seat or not.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is plan; otherwise, <c>false</c>..
    /// </value>
    public bool IsPerUserPlan { get; set; }

}