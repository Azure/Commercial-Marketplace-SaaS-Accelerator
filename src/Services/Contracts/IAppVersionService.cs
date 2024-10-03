using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// App Version Service.
/// </summary>
public interface IAppVersionService
{
    string Version { get; }
}