using System;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

/// <summary>
/// URL Validator.
/// </summary>
public class UrlValidator
{
    /// <summary>
    /// Validates the URL for HTTPS.
    /// Helps validate if the URL is a valid HTTPS URL.
    /// </summary>
    public static bool IsValidUrlHttps(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && 
               uriResult.Scheme == Uri.UriSchemeHttps && 
               uriResult.Port == 443;
    }
}