namespace MyJour.Models
{
    public class Timetable
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public string Day { get; set; }
        public int LessonTimeId { get; set; }
        public Subject Subject { get; set; }
        public Class Class { get; set; }
        public LessonTime LessonTime { get; set; }
    }
}
