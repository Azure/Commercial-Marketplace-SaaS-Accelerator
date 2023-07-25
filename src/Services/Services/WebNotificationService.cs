using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to send emails using SMTP settings.
/// </summary>
/// <seealso cref="IWebNotificationService" />
public class WebNotificationService : IWebNotificationService
{

    /// <summary>
    /// Defines the  API Client.
    /// </summary>
    private readonly IFulfillmentApiService apiService;

    /// <summary>
    /// Defines the  Application Config Repo.
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// Defines the  SaaSAPI client configuration.
    /// </summary>
    private SaaSApiClientConfiguration saaSApiClientConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebNotificationService"/> class.
    /// </summary>
    /// <param name="apiService">apiService.</param>
    /// <param name="applicationConfigRepository">applicationConfigRepository.</param>
    /// <param name="saaSApiClientConfiguration">saaSApiClientConfiguration.</param>
    public WebNotificationService(IFulfillmentApiService apiService,
                                  IApplicationConfigRepository applicationConfigRepository,
                                  SaaSApiClientConfiguration saaSApiClientConfiguration)
    {
        this.apiService = apiService;
        this.applicationConfigRepository = applicationConfigRepository;
        this.saaSApiClientConfiguration = saaSApiClientConfiguration;
    }

    /// <summary>
    /// Prepare the notification payload from landing page.
    /// </summary>
    /// <param name="SubscriptiondId">Subscription Id.</param>
    /// <param name="SubscriptionParameters">Subscription Parameters.</param>
    public async Task PushExternalWebNotificationAsync(Guid SubscriptiondId, List<SubscriptionParametersModel> SubscriptionParameters)
    {
        WebNotificationPayloadLandingPage webNotificationLandingpagePayload = new WebNotificationPayloadLandingPage()
        {
            WebNotificationCustomInfo = new WebNotificationCustomInfo()
            {
                ApplicationName = this.applicationConfigRepository.GetValueByName("ApplicationName"),
                EventType = "LandingPage",
                LandingPageCustomFields = SubscriptionParameters?
                                            .Select(subparam => new KeyValuePair<string, string>(subparam.DisplayName, subparam.Value))?
                                            .ToList()
            },
        };
        var getSubApiResult = await this.apiService.GetSubscriptionByIdAsync(SubscriptiondId).ConfigureAwait(false);
        webNotificationLandingpagePayload.PayloadFromMarketplace = new WebNotificationSubscription
        {
            Id = getSubApiResult.Id,
            PublisherId = getSubApiResult.PublisherId,
            OfferId = getSubApiResult.OfferId,
            Name = getSubApiResult.Name,
            SaasSubscriptionStatus = getSubApiResult.SaasSubscriptionStatus,
            PlanId = getSubApiResult.PlanId,
            Quantity = getSubApiResult.Quantity,
            Purchaser = new PurchaserResult
            {
                EmailId = getSubApiResult.Purchaser.EmailId,
                TenantId = getSubApiResult.Purchaser.TenantId,
                ObjectId = getSubApiResult.Purchaser.ObjectId,
            },
            Beneficiary = new BeneficiaryResult
            {
                EmailId = getSubApiResult.Beneficiary.EmailId,
                TenantId = getSubApiResult.Beneficiary.TenantId,
                Puid = getSubApiResult.Beneficiary.Puid,
                ObjectId = getSubApiResult.Beneficiary.ObjectId,
            },
            Term = new TermResult
            {
                EndDate = getSubApiResult.Term.EndDate,
                StartDate = getSubApiResult.Term.StartDate,
                TermUnit = getSubApiResult.Term.TermUnit,
            },
        };

        string landingPagePayloadJson = JsonSerializer.Serialize(webNotificationLandingpagePayload, new JsonSerializerOptions() { WriteIndented = true });
        await CallNotificationURL(landingPagePayloadJson);
    }

    /// <summary>
    /// Prepare the webhook notification payload.
    /// </summary>
    /// <param name="WebhookPayload">Content of the Webhook Payload.</param>
    public async Task PushExternalWebNotificationAsync(WebhookPayload WebhookPayload)
    {
        WebNotificationPayloadWebhook webNotificationWebhookPayload = new WebNotificationPayloadWebhook()
        {
            WebNotificationCustomInfo = new WebNotificationCustomInfo()
            {
                ApplicationName = this.applicationConfigRepository.GetValueByName("ApplicationName"),
                EventType = WebhookPayload.Action.ToString()
            },
            PayloadFromMarketplace = WebhookPayload
        };
        string webhookPayloadJson = JsonSerializer.Serialize(webNotificationWebhookPayload, new JsonSerializerOptions() { WriteIndented = true });

        await CallNotificationURL(webhookPayloadJson);
    }


    /// <summary>
    /// Sends out the notification.
    /// </summary>
    /// <param name="payload">Content of the notification.</param>
    async Task CallNotificationURL(string payload)
    {
        try
        {
            if (!String.IsNullOrWhiteSpace(saaSApiClientConfiguration.WebNotificationUrl))
            {
                using (var httpClient = new HttpClient())
                {
                    // Create a StringContent object with the payload
                    var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

                    // Send a POST request to the webhook URL
                    var response = await httpClient.PostAsync(saaSApiClientConfiguration.WebNotificationUrl, content);

                    // Check the response status code
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Notification sent successfully");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send notification. Status code: {response.StatusCode}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"No notification sent. Webhook notification URL is empty");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending the notification: {ex.Message}");
        }
    }


}