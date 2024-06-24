using System.Linq;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
    private readonly ApplicationConfigService applicationConfigService;

    private readonly IAppVersionService appVersionService;
    public BaseController(IApplicationConfigRepository applicationConfigRepository, 
                          IAppVersionService appVersionService)
    {
        this.applicationConfigService = new ApplicationConfigService(applicationConfigRepository);
        this.CheckAuthentication();
        this.appVersionService = appVersionService;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (TempData is not null)
        {
            bool.TryParse(this.applicationConfigService.GetValueByName("IsMeteredBillingEnabled"), out bool supportMeteredBilling);
            if (supportMeteredBilling)
            {
                TempData["SupportMeteredBilling"] = "1";
            }
            else
            {
                TempData["SupportMeteredBilling"] = "0";
            }
        }
        ViewData["AppVersion"] = appVersionService?.Version;
        base.OnActionExecuting(filterContext);
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
    /// <returns>
    /// Current Logged User Email.
    /// </returns>
    public PartnerDetailViewModel GetCurrentUserDetail()
    {
        if (this.HttpContext != null && this.HttpContext.User.Identity.IsAuthenticated)
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


    /// <summary>
    /// Gets the Application Version
    /// </summary>
    /// <value>
    /// The name of the Application Version.
    /// </value>
    public string GetAppReleaseVersion()
    {
        return this.appVersionService?.Version;
    }
}