using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// SA Git Release Service.
/// </summary>
public interface ISAGitReleasesService
{
    /// <summary>
    /// Gets the latest release number.
    /// </summary>
    /// <returns> Release Version.</returns>
    public string GetLatestReleaseFromGitHub();
}