using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyJour.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}
        public ICollection<Subject> Subject { get; set; }
        public ICollection<Teacher> Teacher { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Homework> Homework { get; set; }
        public ICollection<Plan> Plan { get; set; }
        static public List<SelectListItem> GetAllClasses(ApplicationDbContext db)
        {
            var list = db.Class.Select(s => new { s.Id, s.Number });
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                classes.Add(new SelectListItem { Text = item.Number.ToString(), Value = item.Id.ToString() });
            }
            return classes;
        }
    }
}
