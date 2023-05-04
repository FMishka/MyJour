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
            if (userRole != null)
            {
                if (_roles == "All" || _roles.Contains(userRole)) // Разрешение всем авторизированным пользователям, либо определённым ролям
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
