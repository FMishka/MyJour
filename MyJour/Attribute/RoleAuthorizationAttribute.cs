using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace MyJour.Attribute
{
    public class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string _roles;
        private readonly bool _isTeacherMode;

        public RoleAuthorizationAttribute(string isTeacherMode, params string[] roles)
        {
            _isTeacherMode = Convert.ToBoolean(isTeacherMode);
            _roles = String.Join(",", roles);
        }
        public RoleAuthorizationAttribute( params string[] roles)
        {
            _roles = String.Join(",", roles);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userRole = context.HttpContext.Session.GetString("Role");
            bool isTeacher;
            if (_isTeacherMode) //Если включена проверка на учителя, то проверяем. Нет - не проверяем
            {
                isTeacher = Convert.ToBoolean(context.HttpContext.Session.GetString("IsTeacher"));
            }
            else
            {
                isTeacher = false;
            }
            if (userRole != null)
            {
                if (_roles == "All" || _roles.Contains(userRole) || isTeacher) // Разрешение всем авторизированным пользователям, либо определённым ролям
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
