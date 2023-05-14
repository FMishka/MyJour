namespace MyJour.Models
{
    public class Student : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int ClassId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Class Class { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set; }
        public ICollection<StudentsByParents> StudentsByParents { get; set; }
    }
}
