namespace MyJour.Models
{
    public class AcademicPerformance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int Grade { get; set; }
        public int TypeControlId { get; set; }
        public Student Student { get; set; }
        public Class Class { get; set; }
        public Subject Subject { get; set; }
        public TypeControl TypeControl { get; set; }
        static public List<DateTime> GetAllDatesAndInitializeTickets(DateTime startingDate, DateTime endingDate)
        {
            List<DateTime> allDates = new List<DateTime>();

            for (DateTime date = startingDate; date <= endingDate; date = date.AddDays(1))
                allDates.Add(date);

            return allDates;
        }
    }
}
