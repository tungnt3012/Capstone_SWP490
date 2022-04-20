using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Capstone_SWP490.Sercurity
{
    public class AuthorizationAccept : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectResult("~/Authentication");
            }
        }
    }
}