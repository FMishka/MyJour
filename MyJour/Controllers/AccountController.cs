using Microsoft.AspNetCore.Mvc;
using MyJour.Models;

namespace MyJour.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;

        public AccountController(ApplicationDbContext context)
        {
            db = context;
        }

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

            if (authenticatedUser != null)
            {
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Parent.Select(u => new { u.Login, u.Password }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                return RedirectToAction("Index", "Home");
            }
            else if ((authenticatedUser = db.Student.Select(u => new { u.Login, u.Password }).FirstOrDefault(u => u.Login == user.Login && u.Password == user.Password)) != null)
            {
                HttpContext.Session.SetString("Login", authenticatedUser.Login);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Неправильное имя пользователя или пароль.";
            return View(user);
        }
    }
}
