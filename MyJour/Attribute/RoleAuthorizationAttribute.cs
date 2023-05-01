using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace MyJour.Attribute
{
    public class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string _roles;

        public RoleAuthorizationAttribute(params string[] roles)
        {
            _roles = String.Join(",", roles);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userRole = context.HttpContext.Session.GetString("Role");
            if (_roles.Contains(userRole))
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
