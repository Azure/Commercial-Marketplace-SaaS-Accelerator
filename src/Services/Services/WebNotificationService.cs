using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Helpers;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Utilities;
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
    /// The application log repository.
    /// </summary>
    private readonly IApplicationLogRepository applicationLogRepository;

    /// <summary>
    /// The application log service.
    /// </summary>
    private readonly ApplicationLogService applicationLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebNotificationService"/> class.
    /// </summary>
    /// <param name="apiService">apiService.</param>
    /// <param name="applicationConfigRepository">applicationConfigRepository.</param>
    /// <param name="saaSApiClientConfiguration">saaSApiClientConfiguration.</param>
    public WebNotificationService(IFulfillmentApiService apiService,
                                  IApplicationConfigRepository applicationConfigRepository,
                                  IApplicationLogRepository applicationLogRepository)
    {
        this.apiService = apiService;
        this.applicationConfigRepository = applicationConfigRepository;
        this.applicationLogRepository = applicationLogRepository;
        this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);

    }

    /// <summary>
    /// Prepare the notification payload from landing page.
    /// </summary>
    /// <param name="SubscriptiondId">Subscription Id.</param>
    /// <param name="SubscriptionParameters">Subscription Parameters.</param>
    public async Task PushExternalWebNotificationAsync(Guid SubscriptiondId, List<SubscriptionParametersModel> SubscriptionParameters)
    {
        var getSubApiResult = await this.apiService.GetSubscriptionByIdAsync(SubscriptiondId).ConfigureAwait(false);

        WebNotificationPayload webNotificationLandingpagePayload = new WebNotificationPayload()
        {
            ApplicationName = this.applicationConfigRepository.GetValueByName("ApplicationName"),
            EventType = WebNotificationEventTypeEnum.LandingPage,
            PayloadFromLandingpage = new WebNotificationSubscription()
            {
                
                LandingPageCustomFields = SubscriptionParameters?
                                            .Select(subparam => new WebNotificationLandingPageParam(subparam.DisplayName, subparam.Value))?
                                            .ToList() ?? new List<WebNotificationLandingPageParam>(),
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
            },
            PayloadFromWebhook = null
        };

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        string landingPagePayloadJson = JsonSerializer.Serialize(webNotificationLandingpagePayload, options);
        await CallNotificationURL(landingPagePayloadJson, "LandingPage", webNotificationLandingpagePayload.PayloadFromLandingpage.Id);
    }

    /// <summary>
    /// Prepare the webhook notification payload.
    /// </summary>
    /// <param name="WebhookPayload">Content of the Webhook Payload.</param>
    public async Task PushExternalWebNotificationAsync(WebhookPayload WebhookPayload)
    {
        WebNotificationPayload webNotificationWebhookPayload = new WebNotificationPayload()
        {
            ApplicationName = this.applicationConfigRepository.GetValueByName("ApplicationName"),
            EventType = WebNotificationEventTypeEnum.Webhook,
            PayloadFromWebhook = WebhookPayload,
        };

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true; 
        string webhookPayloadJson = JsonSerializer.Serialize(webNotificationWebhookPayload, options);

        await CallNotificationURL(webhookPayloadJson, "Webhook", WebhookPayload.SubscriptionId);
    }


    /// <summary>
    /// Sends out the notification.
    /// </summary>
    /// <param name="payload">Content of the notification.</param>
    async Task CallNotificationURL(string payload, string eventType, Guid subscriptionId)
    {
        try
        {
            var WebNotificationUrl = this.applicationConfigRepository.GetValueByName(StringLiteralConstants.WebNotificationUrl);

            //validate the URL
            if (!UrlValidator.IsValidUrlHttps(WebNotificationUrl))
            {
                await this.applicationLogService.AddApplicationLog($"{StringLiteralConstants.WebNotificationUrl}: URI is not valid, does not use HTTPS scheme, or uses a port other than 443. No notification forwarded").ConfigureAwait(false);
                return;
            }

            if (!String.IsNullOrWhiteSpace(WebNotificationUrl))
            {
                using (var httpClient = new HttpClient())
                {
                    // Create a StringContent object with the payload
                    var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

                    // Send a POST request to the webhook URL
                    var response = await httpClient.PostAsync(WebNotificationUrl, content);
                    // Check the response status code
                    if (response.IsSuccessStatusCode)
                    {
                        await this.applicationLogService.AddApplicationLog($"{StringLiteralConstants.WebNotificationUrl}: Web notification successfully pushed from {eventType} for {subscriptionId}").ConfigureAwait(false);
                    }
                    else
                    {
                        await this.applicationLogService.AddApplicationLog($"{StringLiteralConstants.WebNotificationUrl}: Failed to push web notification from {eventType} for {subscriptionId}. Status code: {response.StatusCode}").ConfigureAwait(false);
                    }
                }
            }
            else
            {
                await this.applicationLogService.AddApplicationLog($"{StringLiteralConstants.WebNotificationUrl}: No notification pushed. Webhook notification URL is empty");
            }
        }
        catch (Exception)
        {
            await this.applicationLogService.AddApplicationLog($"{StringLiteralConstants.WebNotificationUrl}: An error occurred while pushing the notification for {subscriptionId}");
        }
    }


}