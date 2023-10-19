using System;
using System.Text.RegularExpressions;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

public partial class KeyVaultReferenceResolver
{
    private struct KeyVaultReference
    {
        public string VaultName { get; }
        public string SecretName { get; }

        public KeyVaultReference(string vaultName, string secretName)
        {
            VaultName = vaultName;
            SecretName = secretName;
        }
    }

    private static readonly Regex KeyVaultReferenceRegex = new("^@Microsoft\\.KeyVault\\(VaultName=(?<vaultName>[^,]+);SecretName=(?<secretName>[^,]+)\\)$");

    public static string ProcessKeyVaultReference(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            return secret;

        if (secret.StartsWith("@Microsoft.KeyVault"))
        {
            var kv = ParseKeyVaultReferenceUsingRegex(secret);
            return GetSecretFromKeyVault(kv);
        }

        return secret;
    }

    private static KeyVaultReference ParseKeyVaultReferenceUsingRegex(string kvReference)
    {
        var re = KeyVaultReferenceRegex.Match(kvReference);
        return new KeyVaultReference(re.Groups["vaultName"].Value, re.Groups["secretName"].Value);
    }

    private static string GetSecretFromKeyVault(KeyVaultReference kv)
    {
        var kvUri = "https://" + kv.VaultName + ".vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        return client.GetSecret(kv.SecretName).Value.Value;
    }
}
