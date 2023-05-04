using Microsoft.AspNetCore.Mvc;
using MyJour.Attribute;
using MyJour.Models;
using System.Diagnostics;

namespace MyJour.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [RoleAuthorization("All")]
        public IActionResult Journal()
        {
            var academicPerformance = db.AcademicPerfomance.Where(a => a.ClassId == 1 && a.SubjectId == 1).ToList(); // Еденицы это заглушки, позже будет сортировка по этим данным, ещё и даты добавить
            return View(academicPerformance);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}