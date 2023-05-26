namespace MyJour.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public string Theme { get; set; }
        public DateTime Date { get; set; }
        public Subject Subject { get; set; }
        public Class Class { get; set; }
    }
}
