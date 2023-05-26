using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyJour.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public Teacher Teacher { get; set; }
        public Class Class { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}
        public ICollection<Homework> Homework { get; set;}
        public ICollection<Plan> Plan { get; set;}
        static public List<SelectListItem> GetAllSubjects(ApplicationDbContext db)
        {
            var list = db.Subject.Select(s => new { s.Id, s.Name });
            List<SelectListItem> classes = new List<SelectListItem>();
            foreach (var item in list)
            {
                classes.Add(new SelectListItem { Text = item.Name.ToString(), Value = item.Id.ToString() });
            }
            return classes;
        }
    }
}
