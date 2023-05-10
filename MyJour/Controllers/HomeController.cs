﻿using Microsoft.AspNetCore.Mvc;
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
        //передать соответствующим классам. типо передаёшь подключение к бд и данные, а функция возвращает ассоциативный список
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
            if(HttpContext.Session.GetString("Login") != null)
            {
                ViewBag.Role = "";
                ViewBag.Name = HttpContext.Session.GetString("Name");
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

                    
                }
                else if(HttpContext.Session.GetString("Role") == "Parent")
                {
                    ViewBag.Role = "Parent";
                    var studentByParent = db.StudentsByParents.Include(s => s.Student).Where(s => s.ParentId == Convert.ToInt32(HttpContext.Session.GetString("Id"))).Distinct().ToList();
                    List<string> students = new List<string>();
                    foreach (var item in studentByParent)
                    {
                        students.Add(item.Student.Name);
                    }
                    ViewBag.Students = students;
                }
                else
                {
                    ViewBag.Role = "Student";
                    string login = HttpContext.Session.GetString("Login");
                    var classNumber = db.Student.Include(a => a.Class).Where(s => s.Login == login).FirstOrDefault().Class.Number;
                    ViewBag.Class = classNumber;
                    var studentByParent = db.StudentsByParents.Include(s => s.Parent).Where(s => s.StudentId == Convert.ToInt32(HttpContext.Session.GetString("Id"))).Distinct().ToList();
                    List<string> parents = new List<string>();
                    foreach (var item in studentByParent)
                    {
                        parents.Add(item.Parent.Name);
                    }
                    ViewBag.Parents = parents;
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

            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));

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
        }
        [RoleAuthorization("True", "All")]
        public IActionResult RateStudent()
        {
            return View();
        }
        public IActionResult Homework()
        {
            return View();
        }
        public IActionResult SetHomework()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}