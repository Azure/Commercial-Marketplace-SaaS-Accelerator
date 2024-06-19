using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// SAGitReleasesService Service.
/// </summary>
public class SAGitReleasesService : ISAGitReleasesService
{
    /// <summary>
    /// Gets the latest release number.
    /// </summary>
    /// <returns> Release Version.</returns>
    public async Task<string> GetLatestRelease()
    {
        using (var httpClient = new HttpClient())
        {
            // GitHub API requires a user agent
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            var WebNotificationUrl = "https://api.github.com/repos/Azure/Commercial-Marketplace-SaaS-Accelerator/releases/latest";
            // Send a GET request to the github api URL
            var response = await httpClient.GetAsync(WebNotificationUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var releaseInfo = JsonSerializer.Deserialize<JsonElement>(content);
                var tagName = releaseInfo.GetProperty("tag_name").GetString();
                return tagName ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
    }


}