using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Marketplace.SaaS.Accelerator.Services.Utilities;
public class ValidateJwtTokenAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Retrieve the authorization header
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

       // string token = "your_jwt_token_here";
        string issuer = "your_issuer_here";
        string audience = "your_audience_here";
        string secretKey = "your_secret_key_here";

        // For demonstration purposes, let's just check if the token is valid
        bool tokenIsValid = ValidateToken(token, issuer, audience, secretKey);

        if (!tokenIsValid)
        {
            context.Result = new UnauthorizedResult();
        }


    }

    private bool ValidateToken(string token, string issuer, string audience, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters(issuer, audience, secretKey);

        try
        {
            SecurityToken validatedToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true; // Token is valid
        }
        catch (SecurityTokenException)
        {
            return false; // Token validation failed
        }
    }

    private TokenValidationParameters GetValidationParameters(string issuer, string audience, string secretKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey)),

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    }
}
