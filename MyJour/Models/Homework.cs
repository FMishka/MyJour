namespace MyJour.Models
{
    public class Homework
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string Task { get; set; }
        public DateTime Deadline { get; set; }
        public Class Class { get; set; }
        public Subject Subject { get; set; }
    }
}
