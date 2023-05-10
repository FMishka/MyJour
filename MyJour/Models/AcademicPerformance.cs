using System.Globalization;

namespace MyJour.Models
{
    public class AcademicPerformance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int Grade { get; set; }                          // Возможно сделать str чтобы ставить тут посещаемость
        public int TypeControlId { get; set; }
        public DateTime Date { get; set; }
        public Student Student { get; set; }
        public Class Class { get; set; }
        public Subject Subject { get; set; }
        public TypeControl TypeControl { get; set; }
    }
}
