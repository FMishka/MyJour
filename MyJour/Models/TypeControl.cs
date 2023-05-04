namespace MyJour.Models
{
    public class TypeControl
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<AcademicPerformance> AcademicPerformance { get; set;}

    }
}
