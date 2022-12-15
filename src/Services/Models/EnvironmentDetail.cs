namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Environment Detail.
/// </summary>
public class EnvironmentDetail
{
    /// <summary>
    /// Gets or sets the name of the azure user.
    /// </summary>
    /// <value>
    /// The name of the azure user.
    /// </value>
    public string AzureUserName { get; set; }

    /// <summary>
    /// Gets or sets the azure password.
    /// </summary>
    /// <value>
    /// The azure password.
    /// </value>
    public string AzurePassword { get; set; }

    /// <summary>
    /// Gets or sets the azure client identifier.
    /// </summary>
    /// <value>
    /// The azure client identifier.
    /// </value>
    public string AzureClientId { get; set; }
}