using Microsoft.Marketplace.SaaS.SDK.Services.WebHook;
using Microsoft.Marketplace.SaasKit.Client.Controllers.WebHook;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

namespace SaaS.SDK.CustomerProvisioning.Test;

public class AzureWebhookControllerTest
{
    private readonly AzureWebhookController _controller;

    public AzureWebhookControllerTest()
    {
        var moqApplicationLogRepository = new Mock<IApplicationLogRepository>();
        var mockWebhookProcessor = new Mock<IWebhookProcessor>();

        _controller = new AzureWebhookController(moqApplicationLogRepository.Object, mockWebhookProcessor.Object);
    }

    [Fact]
    public async void Post()
    {
        var webhookPayload = new Mock<WebhookPayload>();
        webhookPayload.SetupAllProperties();

        await _controller.Post(webhookPayload.Object);
    }
}