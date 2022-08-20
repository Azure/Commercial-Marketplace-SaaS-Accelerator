// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaSAccelerator.Services.WebHook
{
    /// <summary>
    /// Web hook Processor Interface
    /// </summary>
    public interface IWebhookProcessor
    {
        /// <summary>
        /// Processes the Web hook notification asynchronous.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <returns>Processes the Web hook notification</returns>
        Task ProcessWebhookNotificationAsync(WebhookPayload details);
    }
}
