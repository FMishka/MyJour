namespace MyJour.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        ICollection<Teacher> Teacher { get; set; }
    }
}
