namespace MyJour.Models
{
    public class Teacher : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int ClassId { get; set; }
        public int RoleId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Role Role { get; set; }
        public Class Class { get; set; }
        public ICollection<Subject> Subject { get; set; }

    }
}
