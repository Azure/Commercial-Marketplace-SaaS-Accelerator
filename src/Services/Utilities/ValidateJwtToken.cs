using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using System.Security.Claims;

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
        var audience = _saasapiConfiguration.ClientId;

        var validationParameters = new TokenValidationParameters
        {
            //Issuer can change based on the token version. So we are not validating the issuer
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(0)
        };
        
        var handler = new JwtSecurityTokenHandler();

        //validate aud, expiry and signature using jwt validation handler
        ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

        // Get the 'tid' claim
        Claim tidClaim = claimsPrincipal.FindFirst("tid");
        string tenantId = tidClaim?.Value;

        Claim tidfullClaim = claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");
        string tidfull = tidfullClaim?.Value;

        //For the old token v1, its 'appId' and for v2 its 'azp'. Try get both.
        Claim  azpClaim = claimsPrincipal.FindFirst("azp");
        string azpId = azpClaim?.Value;

        Claim appidClaim = claimsPrincipal.FindFirst("appid");
        string appId = appidClaim?.Value;

        //return false if the tenantId or azpId or appId is not matching with the configuration
        if ((tenantId != _saasapiConfiguration.TenantId) && (tidfull != _saasapiConfiguration.TenantId))
        {
            throw new Exception("TenantId is not matching with the configuration");
        }
        if ((azpId != _saasapiConfiguration.Resource) && (appId != _saasapiConfiguration.Resource))
        {
            throw new Exception("azpId or appId is not matching with the configuration");
        }

        return true;
    }
}
