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

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Login") != null)
            {
                ViewBag.Role = "";
                ViewBag.Name = HttpContext.Session.GetString("Name");
                ViewBag.Phone = "";
                ViewBag.Address = "";
                if (HttpContext.Session.GetString("IsTeacher") == "True")
                {
                    string login = HttpContext.Session.GetString("Login");
                    ViewBag.Role = "Teacher";
                    try
                    {
                        int? classId = null;
                        classId = db.Teacher.Select(s => new { s.Login, s.ClassId }).Where(s => s.Login == login).FirstOrDefault().ClassId;
                        var classNumber = db.Class.Where(c => c.Id == classId).FirstOrDefault();
                        ViewBag.Class = classNumber.Number;
                        ViewBag.RoleFromTable = HttpContext.Session.GetString("Role");
                    }
                    catch (Exception)
                    {
                        ViewBag.RoleFromTable = HttpContext.Session.GetString("Role");
                    }
                    ViewBag.Phone = db.Teacher.Select(s => new { s.Id, s.Phone }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Phone; ;
                    ViewBag.Address = db.Teacher.Select(s => new { s.Id, s.Address }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Address;
                }
                else if(HttpContext.Session.GetString("Role") == "Parent")
                {
                    ViewBag.Role = "Parent";
                    var studentByParent = db.StudentsByParents.Include(s => s.Student.Class)
                        .Where(s => s.ParentId == Convert.ToInt32(HttpContext.Session.GetString("Id")))
                        .Distinct()
                        .ToList();
                    List<string> students = new List<string>();
                    foreach (var item in studentByParent)
                    {
                        students.Add(item.Student.Name + "; Класс, в котором обучается: " + item.Student.Class.Number + "\n");
                    }
                    ViewBag.Students = students;
                    ViewBag.Phone = db.Parent.Select(s => new {s.Id, s.Phone}).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Phone;
                    ViewBag.Address = db.Parent.Select(s => new { s.Id, s.Address }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Address;
                }
                else
                {
                    ViewBag.Role = "Student";
                    string login = HttpContext.Session.GetString("Login");
                    var classNumber = db.Student.Include(a => a.Class).Where(s => s.Login == login).FirstOrDefault().Class.Number;
                    ViewBag.Class = classNumber;
                    var studentByParent = db.StudentsByParents.Include(s => s.Parent)
                        .Where(s => s.StudentId == Convert.ToInt32(HttpContext.Session.GetString("Id")))
                        .Distinct()
                        .ToList();
                    List<string> parents = new List<string>();
                    foreach (var item in studentByParent)
                    {
                        parents.Add(item.Parent.Name);
                    }
                    ViewBag.Parents = parents;
                    ViewBag.Phone = db.Student.Select(s => new { s.Id, s.Phone }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Phone;
                    ViewBag.Address = db.Student.Select(s => new { s.Id, s.Address }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault().Address;
                }
                return View();
            }
            return new RedirectToActionResult("Login", "Account", null);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        //Сделать ограниченную выборку для школьника и опекуна этого школьника
        [RoleAuthorization("True", "All")]
        public IActionResult Journal(int? month, int? year, int? classId, int? subjectId)
        {
            ViewBag.Month = Date.GetAllMonth();
            ViewBag.SelectedMonth = month;
            ViewBag.Year = Date.GetLast2Years();
            ViewBag.SelectedYear = year;
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SelectedClassId = classId;
            ViewBag.SubjectId = Subject.GetAllSubjects(db);
            ViewBag.SelectedSubjectId = subjectId;

            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));
            ViewBag.IsParent = HttpContext.Session.GetString("Role") == "Parent" ? true : false; 

            if (classId != null && subjectId != null)
            {
                var academicPerformance = db.AcademicPerfomance.Include(s => s.Student)
                    .Include(c => c.Class)
                    .Include(sub => sub.Subject)
                    .Where(a => a.ClassId == classId && a.SubjectId == subjectId && a.Date.Month == month && a.Date.Year == year)
                    .ToList();
                ViewBag.Count = academicPerformance.Count;
                return View(academicPerformance);
            }
            else if (subjectId != null && HttpContext.Session.GetString("Role") == "Student")
            {
                var userClassId = db.Student.Select(s => new { s.Id, s.ClassId }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault(); ;
                var academicPerformance = db.AcademicPerfomance.Include(s => s.Student)
                    .Include(c => c.Class)
                    .Include(sub => sub.Subject)
                    .Where(a => a.ClassId == userClassId.ClassId && a.SubjectId == subjectId && a.Date.Month == month && a.Date.Year == year)
                    .ToList();
                ViewBag.Count = academicPerformance.Count;
                return View(academicPerformance);
            }
            return View();
        }
        [RoleAuthorization("True", "All")]
        public IActionResult RateStudent()
        {
            return View();
        }
        [RoleAuthorization("True", "All")]
        public IActionResult Homework(int? classId, int? subjectId)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SubjectId = Subject.GetAllSubjects(db);

            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));
            ViewBag.IsParent = HttpContext.Session.GetString("Role") == "Parent" ? true : false;

            int? currentClassId;
            if (HttpContext.Session.GetString("Role") == "Student")
            {
                var userClassId = db.Student.Select(s => new { s.Id, s.ClassId }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault();
                currentClassId = userClassId.ClassId;
            }
            else
            {
                currentClassId = classId;
            }
            if (ViewBag.SubjectId != null)
            {

                    var homework = db.Homework.Include(a => a.Class).Include(s => s.Subject).Where(s => s.SubjectId == subjectId && s.ClassId == currentClassId).ToList();
                    ViewBag.Count = homework.Count;
                    return View(homework);
                
            }
            return View();
        }
        [RoleAuthorization("True", "All")]
        public IActionResult SetHomework()
        {
            return View();
        }
        [RoleAuthorization("True", "All")]
        public IActionResult Timetable(int? classId, string weekdays)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.Weekdays = Date.GetWeekdays();

            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));
            ViewBag.IsParent = HttpContext.Session.GetString("Role") == "Parent" ? true : false;

            int? currentClassId;
            if (HttpContext.Session.GetString("Role") == "Student")
            {
                var userClassId = db.Student.Select(s => new { s.Id, s.ClassId }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault();
                currentClassId = userClassId.ClassId;
            }
            else
            {
                currentClassId = classId;
            }
            if (ViewBag.Weekdays != null)
            {
                var timetable = db.Timetable.Include(t => t.LessonTime)
                    .Include(a => a.Class)
                    .Include(s => s.Subject)
                    .Where(s =>  s.ClassId == currentClassId && s.Day == weekdays).ToList();
                ViewBag.Count = timetable.Count;
                return View(timetable);
            }


            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}