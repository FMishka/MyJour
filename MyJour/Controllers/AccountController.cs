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
        private void SetUserData(string id, string login, string role, string name, bool isTeacher)
        {
            HttpContext.Session.SetString("Id", id);
            HttpContext.Session.SetString("Login", login);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("IsTeacher", isTeacher.ToString());
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Login");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("IsTeacher");
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
            var authenticatedUser = db.Teacher.Select(u => new {u.Id, u.Login, u.Password, u.Name}).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
            if (authenticatedUser != null)
            {
                var role = db.Teacher.Select(u => new { u.Login, u.Password, u.Role.Name }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password);
                SetUserData(authenticatedUser.Id.ToString(), authenticatedUser.Login, role.Name, authenticatedUser.Name, true);
                return RedirectToAction("Journal", "Home");
            }
            else if ((authenticatedUser = db.Parent.Select(u => new { u.Id, u.Login, u.Password, u.Name}).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                SetUserData(authenticatedUser.Id.ToString(), authenticatedUser.Login, "Parent", authenticatedUser.Name, false);
                return RedirectToAction("Journal", "Home");
            }
            else if ((authenticatedUser = db.Student.Select(u => new { u.Id, u.Login, u.Password, u.Name }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                SetUserData(authenticatedUser.Id.ToString(), authenticatedUser.Login, "Student", authenticatedUser.Name, false);
                return RedirectToAction("Journal", "Home");
            }

            ViewBag.ErrorMessage = "Неправильное имя пользователя или пароль.";
            return View(user);
        }

    }
}
