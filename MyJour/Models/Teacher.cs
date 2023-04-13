namespace MyJour.Models
{
    public class Teacher : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int ClassId { get; set; }
        public int RoleId { get; set; }
    }
}
