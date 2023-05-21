namespace MyJour.Models
{
    public class LessonTime
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public ICollection<Timetable> Timetable { get; set; }
    }
}
