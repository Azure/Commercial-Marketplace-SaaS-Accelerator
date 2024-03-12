using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;

namespace Marketplace.SaaS.Accelerator.Services.Utilities;

public class ValidateJwtToken
{

    private readonly SaaSApiClientConfiguration _saasapiConfiguration;

    public ValidateJwtToken(SaaSApiClientConfiguration saasapiConfiguration)
    {
        _saasapiConfiguration = saasapiConfiguration;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
           $"https://login.microsoftonline.com/{_saasapiConfiguration.TenantId}/.well-known/openid-configuration",
           new OpenIdConnectConfigurationRetriever(),
           new HttpDocumentRetriever());
        var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
        var signingKeys = openIdConfig.SigningKeys;

        var issuer = $"https://login.microsoftonline.com/{_saasapiConfiguration.TenantId}/v2.0";
        var audience = _saasapiConfiguration.ClientId;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateAudience = true,
            ValidAudience = audience
        };
        
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(token, validationParameters, out var validatedToken);
        return true;
    }
}
