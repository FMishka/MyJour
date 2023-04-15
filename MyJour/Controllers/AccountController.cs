using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyJour.Models;
using MyJour.Attribute;

namespace MyJour.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;

        public AccountController(ApplicationDbContext context)
        {
            db = context;
        }
        [RoleAuthorization("Teacher")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                //db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            var authenticatedUser = db.Teacher.Select(u => new {u.Login, u.Password}).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            //Скорее всего можнно упрознить User и просто выдавать роль только через Role
            if (authenticatedUser != null)
            {
                var role = db.Teacher.Where(u => u.Login == user.Login).Select(u => u.Role.Name);
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                HttpContext.Session.SetString("User", "Teacher");
                foreach (var item in role)
                {
                    HttpContext.Session.SetString("Role", item); // потомпоменять
                }
                

                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Parent.Select(u => new { u.Login, u.Password }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                HttpContext.Session.SetString("User", "Parent");
                HttpContext.Session.SetString("Role", "User");
                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Student.Select(u => new { u.Login, u.Password }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                HttpContext.Session.SetString("User", "Student");
                HttpContext.Session.SetString("Role", "User");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Неправильное имя пользователя или пароль.";
            return View(user);
        }
    }
}
