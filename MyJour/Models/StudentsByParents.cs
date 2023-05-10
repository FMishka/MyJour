namespace MyJour.Models
{
    public class StudentsByParents
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int StudentId { get; set;}
        public Student Student { get; set; }
        public Parent Parent { get; set; }
    }
}
