using System;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

/// <summary>
/// NewSAVersionCheckHelper will check if check for any latest version for Saas Accelerator
/// </summary>
public class NewSAVersionCheckHelper
{
    /// <summary>
    /// Compares the latest version of SA with the current version.
    /// </summary>
    public static bool? IsNewVersionAvailable(string SALatestVersionFromGithub = "1.0.0", string currentVersion = "1.0.0")
    {
        try
        {
            // Split the version strings into their numeric components
            string[] currentVersionParts = currentVersion.Split('.');
            string[] newVersionParts = SALatestVersionFromGithub.Split('.');

            // Determine the length of the shortest version string
            int length = Math.Min(currentVersionParts.Length, newVersionParts.Length);

            // Compare each component of the version numbers
            for (int i = 0; i < length; i++)
            {
                int currentVersionPart = int.Parse(currentVersionParts[i]);
                int newVersionPart = int.Parse(newVersionParts[i]);

                if (newVersionPart > currentVersionPart)
                {
                    return true;
                }
                else if (newVersionPart < currentVersionPart)
                {
                    return false;
                }
            }

            // If the common parts are the same, check if the new version has additional parts
            return newVersionParts.Length > currentVersionParts.Length;
        }
        catch (Exception)
        {
            return null;
        }
    }
}