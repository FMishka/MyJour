using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace MyJour.Attribute
{
    public class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public RoleAuthorizationAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Проверяем наличие роли или других критериев авторизации
            string userRole = context.HttpContext.Session.GetString("User"); // Получаем значение из сессии
            if (userRole != _role)
            {
                // Если роль не соответствует ожидаемой роли, перенаправляем на начальную страницу
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
