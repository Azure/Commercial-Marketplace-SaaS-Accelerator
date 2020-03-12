using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Marketplace.Saas.Web.Utilities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

namespace Microsoft.Marketplace.SaaS.SDK.PublisherSolution.Utilities
{
    public class KnownUser : AuthorizeAttribute, IAuthorizationFilter
    {

        /// <summary>
        /// The known users repository
        /// </summary>
        //private readonly IKnownUsersRepository knownUsersRepository;

        //public KnownUser(IKnownUsersRepository KnownUsersRepository)
        //{
        //    knownUsersRepository = KnownUsersRepository;

        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isKnownuser = false;
            string email = "";
            if (context.HttpContext != null && context.HttpContext.User.Claims.Count() > 0)
            {
                email = context.HttpContext.User.Claims.Where(s => s.Type == WebConstants.CLAIM_EMAILADDRESS).FirstOrDefault().Value;
                //KnownUsersRepository 
                //bool isKnownUser = knownUsersRepository.GetKnownUserDetail(email, 1)?.Id > 0;
                //if (email == "Phaneendra.Nagubandi@spektrasystems.com")
                if (!isKnownuser)
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues["controller"] = "Account";
                    routeValues["action"] = "AccessDenied";
                    //Other route values if needed.
                    context.Result = new RedirectToRouteResult(routeValues);
                }
            }
            else
            {
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Account";
                routeValues["action"] = "SignIn";
                //Other route values if needed.
                context.Result = new RedirectToRouteResult(routeValues);
            }


        }
    }
}
