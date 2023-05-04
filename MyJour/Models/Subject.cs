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

    }
}
