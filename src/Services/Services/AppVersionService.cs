using System;
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

    /// <summary>
    /// Sets the Application Version.
    /// </summary>
    public AppVersionService(Version version)
    {
        if (version != null)
        {
            Version = $"{version?.Major}.{version?.Minor}.{version?.Build}";
        }
        else
        {
            Version = "1.0.0";
        }
    }
}
