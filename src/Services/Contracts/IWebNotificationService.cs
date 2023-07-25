using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

/// <summary>
/// Contract for Web Notification service
/// </summary>
public interface IWebNotificationService
{

    /// <summary>
    /// Prepare the notification payload from landing page.
    /// </summary>
    /// <param name="SubscriptiondId">Subscription Id.</param>
    /// <param name="SubscriptionParameters">Subscription Parameters.</param>
    Task PushExternalWebNotificationAsync(Guid SubscriptiondId, List<SubscriptionParametersModel> SubscriptionParameters);

    /// <summary>
    /// Prepare the webhook notification payload.
    /// </summary>
    /// <param name="WebhookPayload">Content of the Webhook Payload.</param>
    Task PushExternalWebNotificationAsync(WebhookPayload WebhookPayload);
}