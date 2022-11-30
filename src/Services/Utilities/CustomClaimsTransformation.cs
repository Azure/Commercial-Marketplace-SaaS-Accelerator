using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Marketplace.SaaS.Accelerator.Services.Utilities;

public class CustomClaimsTransformation : IClaimsTransformation
{
    /*NOTE
     The OAuth V2 Azure AD provider will send the name under the "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" claim,
       however the OpenId Connect library that maps those claims to the User.Identity object in dotnet, looks for "name" as the claim Type to read the name from
     
     Setting the AddOpenIdConnect -> options.TokenValidationParameters.NameClaimType to the proper values does not seem to work 
     
     This transformation is responsible for copying the value of the Name claim that comes in the token
       under a claim_type of "name" so that it can be propely mapped by the User.Identity object.
    */

    const string ExpectedNameClaimType = "name";

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;

        if (identity.HasClaim(claim => claim.Type == ExpectedNameClaimType))
            return Task.FromResult(principal);

        var nameClaimValue = identity?.Claims.FirstOrDefault(c => c.Type == ClaimConstants.CLAIM_NAME)?.Value;
        if (!string.IsNullOrEmpty(nameClaimValue))
            identity.AddClaim(new Claim(ExpectedNameClaimType, nameClaimValue));
            
        return Task.FromResult(principal);
    }
}