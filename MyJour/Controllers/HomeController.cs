using Microsoft.AspNetCore.Components;
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
        [RoleAuthorization("True", "All")]
        public IActionResult Journal()
        {
            ViewBag.Month = Date.GetAllMonth();
            ViewBag.Year = Date.GetLast2Years();
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SubjectId = Subject.GetAllSubjects(db);
            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));
            ViewBag.IsParent = HttpContext.Session.GetString("Role") == "Parent" ? true : false;
            return View();
        }
        [HttpPost]
        [ActionName("Journal")]
        [RoleAuthorization("True", "All")]
        public IActionResult JournalPost(int? month, int? year, int? classId, int? subjectId)
        {
            if (Request.Form["confirm"] == "Подтвердить")
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
                    ViewBag.Students = db.Student.Where(s => s.ClassId == classId).OrderBy(s => s.Name).ToList();
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

                    var userClassId = db.Student.Select(s => new { s.Id, s.ClassId }).Where(s => s.Id == Convert.ToInt32(HttpContext.Session.GetString("Id"))).FirstOrDefault();
                    ViewBag.Students = db.Student.Where(s => s.ClassId == userClassId.ClassId).OrderBy(s => s.Name).ToList();
                    var academicPerformance = db.AcademicPerfomance.Include(s => s.Student)
                        .Include(c => c.Class)
                        .Include(sub => sub.Subject)
                        .Where(a => a.ClassId == userClassId.ClassId && a.SubjectId == subjectId && a.Date.Month == month && a.Date.Year == year)
                        .ToList();
                    ViewBag.Count = academicPerformance.Count;
                    return View(academicPerformance);
                }
            }
            
            for (int i = 1; i <= DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)); i++)
            {
                if(string.IsNullOrEmpty(Request.Form[$"{i}"]) == false)
                {
                    var students = db.Student.Select(s => s.Id);
                    string grades = Request.Form[$"{i}"].ToString();
                    foreach (var garde in grades.Split("/"))
                    {
                        foreach (var item in students)
                        {
                            if (string.IsNullOrEmpty(Request.Form[$"-{item}"]) == false)
                            {
                                Console.WriteLine("!!" + item);
                                var academicPerformance = new AcademicPerformance
                                {
                                    ClassId = (int)classId,
                                    SubjectId = (int)subjectId,
                                    StudentId = item,
                                    Grade = Convert.ToInt32(garde),
                                    TypeControlId = 1,
                                    Date = new DateTime((int)year, (int)month, i)
                                };
                                db.Add(academicPerformance);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Journal");
        }
        [RoleAuthorization("True","All")]
        [Microsoft.AspNetCore.Mvc.Route("UpdateStudentGrade/{id}")]
        public IActionResult UpdateStudentGrade(string id, string submit, string grades, int typeControlId, DateTime date)
        {
            ViewBag.IsTeacher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));
            string[] gradeIds;
            gradeIds = id.Split(';');
            string[] studentGrades = new string[gradeIds.Length];
            var academicPerformance = db.AcademicPerfomance.AsNoTracking().Include(t => t.TypeControl).FirstOrDefault(s => s.Id == Convert.ToInt32(gradeIds[0]));
            for(int i = 0; i < gradeIds.Length; i++)
            {
                var grade = db.AcademicPerfomance.AsNoTracking().FirstOrDefault(s => s.Id == Convert.ToInt32(gradeIds[i]));
                studentGrades[i] = grade.Grade.ToString();
            }
            ViewBag.Grades = string.Join("/", studentGrades);
            if (ViewBag.IsTeacher == false)
            {
                ViewBag.Type = academicPerformance.TypeControl.Type;
            }
            else
            {
                ViewBag.TypeControlId = TypeControl.GetAllTypesControl(db, academicPerformance.TypeControlId);
            }
            
            ViewBag.Date = academicPerformance.Date;

            if(submit == "Подтвердить")
            {
                foreach (var grade in studentGrades)
                {
                    int i = 0;
                    AcademicPerformance academic = new AcademicPerformance
                    {
                        Id = Convert.ToInt32(gradeIds[i]),
                        StudentId = academicPerformance.StudentId,
                        ClassId = academicPerformance.ClassId,
                        SubjectId = academicPerformance.SubjectId,
                        Grade = Convert.ToInt32(grade),
                        TypeControlId = typeControlId,
                        Date = date
                    };
                    db.Entry(academic).State = EntityState.Modified;
                    db.SaveChanges();
                    i++;
                    Console.WriteLine(i);//надо трекинг убрать
                }
                return RedirectToAction("Journal");
            }

            return View();
        }

        [RoleAuthorization("True")]
        [Microsoft.AspNetCore.Mvc.Route("RateStudent/{studentId}/{classId}/{subjectId}")]
        public IActionResult RateStudent(int studentId, int classId, int subjectId, string grade1, string grade2, int typeControlId, DateTime date)
        {
            ViewBag.TypeControlId = TypeControl.GetAllTypesControl(db);
            try
            {
                if (grade1 != "" && grade1 != null && Convert.ToInt32(grade1) <= 5 && Convert.ToInt32(grade1) >= 0)
                {
                    if (grade2 != "" && grade2 != null && Convert.ToInt32(grade2) <= 5 && Convert.ToInt32(grade2) >= 0)
                    {
                        var academicPerformance1 = new AcademicPerformance { ClassId = classId, SubjectId = subjectId, StudentId = studentId, Grade = Convert.ToInt32(grade1), TypeControlId = typeControlId, Date = date };
                        var academicPerformance2 = new AcademicPerformance { ClassId = classId, SubjectId = subjectId, StudentId = studentId, Grade = Convert.ToInt32(grade2), TypeControlId = typeControlId, Date = date };
                        db.Add(academicPerformance1);
                        db.SaveChanges();
                        db.Add(academicPerformance2);
                        db.SaveChanges();
                        return RedirectToAction("Journal");
                    }
                    else
                    {
                        int grade = 0;
                        if (grade1 == "н")
                        {
                            grade = 0;
                        }
                        else
                        {
                            grade = Convert.ToInt32(grade1);
                        }
                        var academicPerformance1 = new AcademicPerformance { ClassId = classId, SubjectId = subjectId, StudentId = studentId, Grade = grade, TypeControlId = typeControlId, Date = date };
                        db.Add(academicPerformance1);
                        db.SaveChanges();
                        return RedirectToAction("Journal");
                    }
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Error");
            }
            
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
        public IActionResult SetHomework(int classId, int subjectId, string? task, DateTime deadline)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SubjectId = Subject.GetAllSubjects(db);

            ViewBag.IsTeaher = Convert.ToBoolean(HttpContext.Session.GetString("IsTeacher"));

            if (classId != null && subjectId != null && task != null && deadline != null)
            {
                Models.Homework homework = new Homework { ClassId = classId, SubjectId = subjectId, Task = task, Deadline = deadline };
                db.Homework.Add(homework);
                db.SaveChanges();
                
                return RedirectToAction("Homework");
            }

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
        [RoleAuthorization("True")]
        public IActionResult Plan(int subjectId, int classId)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SubjectId = Subject.GetAllSubjects(db);

            var plan = db.Plan.Include(s => s.Class).Include(d => d.Subject).Where(s => s.ClassId == classId && s.SubjectId == subjectId).ToList();

            return View(plan);
        }
        [RoleAuthorization("True")]
        public IActionResult ClassReport(int? classId)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.Avg = 0.0;
            if (classId != null) 
            {
                var academic = db.AcademicPerfomance.Include(s => s.Class).Where(b => b.ClassId == classId).Average(a => a.Grade).ToString();
                ViewBag.Avg = academic;
            }
            
            return View();
        }
        [RoleAuthorization("True")]
        public IActionResult SubjectReport(int? subjectId)
        {
            ViewBag.SubjectId = Subject.GetAllSubjects(db);
            ViewBag.Avg = 0.0;
            if (subjectId != null)
            {
                var academic = db.AcademicPerfomance.Include(s => s.Subject).Where(b => b.SubjectId == subjectId).Average(a => a.Grade).ToString();
                ViewBag.Avg = academic;
            }

            return View();
        }
        [RoleAuthorization("True")]
        public IActionResult StudentReport(int? classId, int? subjectId)
        {
            ViewBag.ClassId = Class.GetAllClasses(db);
            ViewBag.SubjectId = Subject.GetAllSubjects(db);

            ViewBag.List = 0.0;
            if (classId != null && subjectId != null)
            {
                var academic = db.AcademicPerfomance.Include(s => s.Student)
                    .Include(c => c.Class)
                    .Include(sub => sub.Subject).Where(b => b.ClassId == classId && b.SubjectId == subjectId).ToList();
                return View(academic);
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