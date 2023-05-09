using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyJour.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int ClassTypeId { get; set; }
        public ClassType ClassType { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}
        public ICollection<Subject> Subject { get; set; }
        public ICollection<Teacher> Teacher { get; set; }
        public ICollection<Student> Student { get; set; }
    }
}
