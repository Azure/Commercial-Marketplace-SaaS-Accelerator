// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;
    using Microsoft.Marketplace.SaaS.SDK.Services.Attributes;
    using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;

    /// <summary>
    /// Subscription Update Result.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class SubscriptionUpdateResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the operation location.
        /// </summary>
        /// <value>
        /// The operation location.
        /// </value>
        [FromRequestHeader("operation-location")]
        public string OperationIdFromClientLib { get; set; }

        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        /// <exception cref="FulfillmentException">
        /// API did not return an operation ID.
        /// or
        /// URI is not recognized as an operation ID url.
        /// or
        /// Returned operation ID is not a Guid.
        /// </exception>
        [FromRequestHeader("OperationId")]
        public Guid OperationId
        {
            get
            {
                Guid operationGuid;
                if (!Guid.TryParse(OperationIdFromClientLib, out operationGuid))
                {
                    throw new MarketplaceException("Returned operation ID is not a Guid", SaasApiErrorCode.NotFound);
                }

                return operationGuid;
            }
        }
    }
}
