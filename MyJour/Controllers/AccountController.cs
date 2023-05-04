using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyJour.Models;
using MyJour.Attribute;
using System.Data;

namespace MyJour.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;

        public AccountController(ApplicationDbContext context)
        {
            db = context;
        }
        private void SetUserData(string login, string role, string name)
        {
            HttpContext.Session.SetString("Login", login);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Name", name);

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Login");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            var authenticatedUser = db.Teacher.Select(u => new {u.Login, u.Password, u.Name}).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            if (authenticatedUser != null)
            {
                var role = db.Teacher.Select(u => new { u.Login, u.Password, u.Role.Name }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
                SetUserData(authenticatedUser.Login, role.Name, authenticatedUser.Name);
                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Parent.Select(u => new { u.Login, u.Password, u.Name}).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                SetUserData(authenticatedUser.Login, "Parent", authenticatedUser.Name);
                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Student.Select(u => new { u.Login, u.Password, u.Name }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                SetUserData(authenticatedUser.Login, "Student", authenticatedUser.Name);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Неправильное имя пользователя или пароль.";
            return View(user);
        }

    }
}
