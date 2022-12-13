// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Result Extension.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SubscriptionResult" />
public class SubscriptionResultExtension : SubscriptionResult
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is metering supported.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is metering supported; otherwise, <c>false</c>.
    /// </value>
    public bool IsMeteringSupported { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is per user plan.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is per user plan; otherwise, <c>false</c>.
    /// </value>
    public bool IsPerUserPlan { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier plan identifier.
    /// </summary>
    /// <value>
    /// The unique identifier plan identifier.
    /// </value>
    public Guid GuidPlanId { get; set; }

    /// <summary>
    /// Gets or sets the subscription parameters.
    /// </summary>
    /// <value>
    /// The subscription parameters.
    /// </value>
    public List<SubscriptionParametersModel> SubscriptionParameters { get; set; }

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    /// <value>
    /// The name of the event.
    /// </value>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    /// <value>
    /// The subscription status.
    /// </value>
    public SubscriptionStatusEnumExtension SubscriptionStatus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is automatic provisioning supported.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is automatic provisioning supported; otherwise, <c>false</c>.
    /// </value>
    public bool IsAutomaticProvisioningSupported { get; set; }
       
        
    /// <summary>
    /// Gets or sets a value indicating if we allow subscription updates on the customer side.
    /// </summary>
    public bool AcceptSubscriptionUpdates { get; set; }
}