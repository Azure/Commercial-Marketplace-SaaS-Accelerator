namespace Microsoft.Marketplace.SaaS.SDK.Services.Utilities
{
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

    /// <summary>
    /// Authorize attribute to check if the user is a known user.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
    public class KnownUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// The known users repository.
        /// </summary>
        private readonly IKnownUsersRepository knownUsersRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownUserAttribute"/> class.
        /// </summary>
        /// <param name="knownUsersRepository">The known users repository.</param>
        public KnownUserAttribute(IKnownUsersRepository knownUsersRepository)
        {
            this.knownUsersRepository = knownUsersRepository;
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isKnownuser = false;
            string email = string.Empty;
            if (context.HttpContext != null && context.HttpContext.User.Claims.Count() > 0)
            {
                email = context.HttpContext.User.Claims.Where(s => s.Type == ClaimConstants.CLAIM_EMAILADDRESS).FirstOrDefault().Value;
                isKnownuser = this.knownUsersRepository.GetKnownUserDetail(email, 1)?.Id > 0;

                if (!isKnownuser)
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues["controller"] = "Account";
                    routeValues["action"] = "AccessDenied";
                    context.Result = new RedirectToRouteResult(routeValues);
                }
            }
            else
            {
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "SignIn";
                context.Result = new RedirectToRouteResult(routeValues);
            }
        }
    }
}