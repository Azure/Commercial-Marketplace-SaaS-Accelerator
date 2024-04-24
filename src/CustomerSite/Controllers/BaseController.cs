using System.Linq;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers;

/// <summary>
///  Sets a BaseController.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
public class BaseController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    public BaseController()
    {
        this.CheckAuthentication();
    }

    /// <summary>
    /// Gets Current Logged in User Email Address.
    /// </summary>
    /// <value>
    /// The current user email address.
    /// </value>
    public string CurrentUserEmailAddress
    {
        get
        {
            return HttpContext?.User?.Claims?.FirstOrDefault(s => s.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.Value ?? string.Empty; 
        }
    }

    /// <summary>
    /// Gets Current Logged in User Name.
    /// </summary>
    /// <value>
    /// The name of the current user.
    /// </value>
    public string CurrentUserName
    {
        get
        {
            if (this.HttpContext != null && this.HttpContext.User.Claims.Count() > 0)
            {
                var shortNameClaim = this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.CLAIM_SHORT_NAME);
                if (shortNameClaim != null)
                {
                    return shortNameClaim.Value;
                }
                else
                {
                    var fullNameClaim = this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.CLAIM_NAME);
                    if (fullNameClaim != null)
                    {
                        return fullNameClaim.Value;
                    }
                }
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Get Current Logged in User Email Address.
    /// </summary>
    /// <returns> Current Logged User Email.</returns>
    public PartnerDetailViewModel GetCurrentUserDetail()
    {
        if (HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            PartnerDetailViewModel partnerDetail = new PartnerDetailViewModel();
            partnerDetail.FullName = this.CurrentUserName;
            partnerDetail.EmailAddress = this.CurrentUserEmailAddress;
            return partnerDetail;
        }

        return new PartnerDetailViewModel();
    }

    /// <summary>
    /// Checks the authentication.
    /// </summary>
    /// <returns>
    /// Check authentication.
    /// </returns>
    public IActionResult CheckAuthentication()
    {
        if (this.HttpContext == null || !this.HttpContext.User.Identity.IsAuthenticated)
        {
            return this.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }
        else
        {
            return this.RedirectToAction("Index", "Home", new { });
        }
    }
}