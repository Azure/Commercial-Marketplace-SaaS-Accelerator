namespace Microsoft.Marketplace.SaaS.SDK.Client.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class ADAuthenticationResult
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]        
        public string ExpiresIn { get; set; }

        [JsonPropertyName("ext_expires_in")]
        public string ExtExpiresIn { get; set; }

        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }

        [JsonPropertyName("not_before")]
        public string NotBefore { get; set; }

        [JsonPropertyName("resource")]
        public Guid Resource { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
