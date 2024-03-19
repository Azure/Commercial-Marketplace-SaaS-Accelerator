// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.


using System;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers.WebHook;

/// <summary>
/// Azure Web hook.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryTokenAttribute]
public class AzureWebhookController : ControllerBase
{
    /// <summary>
    /// The application log repository.
    /// </summary>
    private readonly IApplicationLogRepository applicationLogRepository;

    /// <summary>
    /// The subscriptions repository.
    /// </summary>
    private readonly ISubscriptionsRepository subscriptionsRepository;

    /// <summary>
    /// The current configuration
    /// </summary>
    private readonly SaaSApiClientConfiguration configuration;

    /// <summary>
    /// The plan repository.
    /// </summary>
    private readonly IPlansRepository planRepository;

    /// <summary>
    /// The subscriptions log repository.
    /// </summary>
    private readonly ISubscriptionLogRepository subscriptionsLogRepository;

    /// <summary>
    /// The web hook processor.
    /// </summary>
    private readonly IWebhookProcessor webhookProcessor;

    /// <summary>
    /// The application log service.
    /// </summary>
    private readonly ApplicationLogService applicationLogService;

    /// <summary>
    /// The subscription service.
    /// </summary>
    private readonly SubscriptionService subscriptionService;

    /// <summary>
    /// The JWT token validation.
    /// </summary>
    private readonly ValidateJwtToken validateJwtToken;

    /// <summary>
    /// The ApplicationConfig Repository.
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// The ApplicationConfig service.
    /// </summary>
    private readonly ApplicationConfigService applicationConfigService;


    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWebhookController"/> class.
    /// </summary>
    /// <param name="applicationLogRepository">The application log repository.</param>
    /// <param name="webhookProcessor">The Web hook log repository.</param>
    /// <param name="subscriptionsLogRepository">The subscriptions log repository.</param>
    /// <param name="planRepository">The plan repository.</param>
    /// <param name="subscriptionsRepository">The subscriptions repository.</param>
    /// <param name="configuration">The SaaSApiClientConfiguration from ENV</param>
    /// <param name="validateJwtToken">The validateJwtToken utility</param>
    /// <param name="applicationConfigRepository">The application config repository</param>
    public AzureWebhookController(IApplicationLogRepository applicationLogRepository, 
                                  IWebhookProcessor webhookProcessor, 
                                  ISubscriptionLogRepository subscriptionsLogRepository, 
                                  IPlansRepository planRepository, 
                                  ISubscriptionsRepository subscriptionsRepository, 
                                  SaaSApiClientConfiguration configuration,
                                  ValidateJwtToken validateJwtToken,
                                  IApplicationConfigRepository applicationConfigRepository)
    {
        this.applicationLogRepository = applicationLogRepository;
        this.subscriptionsRepository = subscriptionsRepository;
        this.configuration = configuration;
        this.planRepository = planRepository;
        this.subscriptionsLogRepository = subscriptionsLogRepository;
        this.webhookProcessor = webhookProcessor;
        this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
        this.subscriptionService = new SubscriptionService(this.subscriptionsRepository, this.planRepository);
        this.validateJwtToken = validateJwtToken;
        this.applicationConfigRepository = applicationConfigRepository;
        this.applicationConfigService = new ApplicationConfigService(this.applicationConfigRepository);
    }

    /// <summary>
    /// Posts the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    public async Task<IActionResult> Post(WebhookPayload request)
    {
        try
        {
            await this.applicationLogService.AddApplicationLog("The azure Webhook Triggered.").ConfigureAwait(false);

            var appConfigValueConversion = bool.TryParse(this.applicationConfigService.GetValueByName("ValidateWebhookJwtToken"), out bool appConfigValue);
            
            if (appConfigValueConversion && appConfigValue)
            {
                try
                {
                    await this.applicationLogService.AddApplicationLog("Validating the JWT token.").ConfigureAwait(false);
                    var token = this.HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];
                    await validateJwtToken.ValidateTokenAsync(token);
                }
                catch (Exception e)
                {
                    await this.applicationLogService.AddApplicationLog($"Jwt token validation failed with error: {e.Message}").ConfigureAwait(false);

                    return new UnauthorizedResult();
                }
            }   

            if (request != null)
            {
                var json = JsonSerializer.Serialize(request);
                await this.applicationLogService.AddApplicationLog("Webhook Serialize Object " + json).ConfigureAwait(false);
                await this.webhookProcessor.ProcessWebhookNotificationAsync(request, configuration).ConfigureAwait(false);
                return Ok();
            }
            throw new MarketplaceException("Request payload is null.");
        }
        catch (MarketplaceException ex)
        {
            await this.applicationLogService.AddApplicationLog(
                    $"A Marketplace exception occurred while attempting to process a webhook notification: [{ex.Message}].")
                .ConfigureAwait(false);
            return BadRequest();
        }
        catch (Exception ex)
        {
            await this.applicationLogService.AddApplicationLog(
                    $"An error occurred while attempting to process a webhook notification: [{ex.Message}].")
                .ConfigureAwait(false);
            return StatusCode(500);
        }
    }
}