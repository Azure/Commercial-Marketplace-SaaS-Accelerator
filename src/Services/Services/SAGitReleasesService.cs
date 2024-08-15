using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// SAGitReleasesService Service.
/// </summary>
public class SAGitReleasesService : ISAGitReleasesService
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<SAGitReleasesService> logger;

    /// <summary>
    public SAGitReleasesService(ILogger<SAGitReleasesService> _logger)
    {
        logger = _logger;   
    }

    /// <summary>
    /// Gets the latest release number.
    /// </summary>
    /// <returns> Release Version.</returns>
    public string GetLatestReleaseFromGitHub()
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                // GitHub API requires a user agent
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                var WebNotificationUrl = "https://api.github.com/repos/Azure/Commercial-Marketplace-SaaS-Accelerator/releases/latest";
                // Send a GET request to the github api URL
                var response = httpClient.GetAsync(WebNotificationUrl).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var releaseInfo = JsonSerializer.Deserialize<JsonElement>(content);
                    var tagName = releaseInfo.GetProperty("tag_name").GetString();
                    return tagName ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Unable to get latest SA release from Github:{ex.Message} :: {ex.InnerException}");
                return string.Empty;
            }
        }
    }


}