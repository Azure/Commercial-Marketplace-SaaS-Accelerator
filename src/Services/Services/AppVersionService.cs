using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// AppVersionService Service.
/// </summary>
public class AppVersionService : IAppVersionService
{

    public string Version { get; }

    public int? ProductMajorPart { get; }

    public int? ProductMinorPart { get; }

    public int? ProductBuildPart { get; }

    /// <summary>
    /// Sets the Application Version.
    /// </summary>
    public AppVersionService(FileVersionInfo version)
    {
        Version = version.ProductVersion;
        ProductMajorPart = version.ProductMajorPart;
        ProductMinorPart = version.ProductMinorPart;
        ProductBuildPart = version.ProductBuildPart;
    }
}