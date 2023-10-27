using System.Linq;
using System.Web;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// Base Controller.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
public class BaseController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController" /> class.
    /// </summary>
    /// 
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
        get { return this.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.Value ?? string.Empty; }
    }

    /// <summary>
    /// Gets Current Logged in User Name.
    /// </summary>
    /// <value>
    /// The name of the current user.
    /// </value>
    public string CurrentUserName
    {
        get { return this.HttpContext?.User.Claims.FirstOrDefault(s => s.Type == ClaimConstants.CLAIM_NAME)?.Value ?? string.Empty; }
    }

    /// <summary>
    /// Get Current Logged in User Email Address.
    /// </summary>
    /// <returns>
    /// Current Logged User Email.
    /// </returns>
    public PartnerDetailViewModel GetCurrentUserDetail()
    {
        if (this.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            return new PartnerDetailViewModel
            {
                FullName = this.CurrentUserName,
                EmailAddress = this.CurrentUserEmailAddress
            };
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
        if (this.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            return this.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }
        return this.RedirectToAction("Index", "Home", new { });
    }

}