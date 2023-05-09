using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyJour.Attribute;
using MyJour.Models;
using System.Diagnostics;

namespace MyJour.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ILogger<HomeController> _logger;

        public List<SelectListItem> GetAllClasses()
        {
            var list = db.Class.Select(s => new { s.Id, s.Number });
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                classes.Add(new SelectListItem { Text = item.Number.ToString(), Value = item.Id.ToString() });
            }
            return classes;
        }
        public List<SelectListItem> GetAllSubjects()
        {
            var list = db.Subject.Select(s => new { s.Id, s.Name });
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                classes.Add(new SelectListItem { Text = item.Name.ToString(), Value = item.Id.ToString() });
            }
            return classes;
        }

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
        //[RoleAuthorization("All")]
        public IActionResult Journal(string month, string year, string classId, string subjectId)
        {
            ViewBag.Month = Date.GetAllMonth();
            ViewBag.SelectedMonth = month;
            ViewBag.Year = Date.GetLast10Years();
            ViewBag.SelectedYear = year;
            ViewBag.ClassId = GetAllClasses();
            ViewBag.SelectedClassId = classId;
            ViewBag.SubjectId = GetAllSubjects();
            ViewBag.SelectedSubjectId = subjectId;
            if (ViewBag.SelectedClassId != null && ViewBag.SelectedSubjectId != null)
            {
                var academicPerformance = db.AcademicPerfomance.Include(s => s.Student)
                                .Include(c => c.Class)
                                .Include(sub => sub.Subject)
                                .Where(a => a.ClassId == Convert.ToInt32(classId) && a.SubjectId == Convert.ToInt32(subjectId) && a.Date.Month == Convert.ToInt32(month) && a.Date.Year == Convert.ToInt32(year))
                                .ToList();
                ViewBag.Count = academicPerformance.Count;
                return View(academicPerformance);
            }
            return View();
             // Еденицы это заглушки, позже будет сортировка по этим данным, ещё и даты добавить
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}