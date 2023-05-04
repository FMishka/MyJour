namespace MyJour.Models
{
    public class ClassType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
