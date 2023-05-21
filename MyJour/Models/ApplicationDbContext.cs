using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyJour.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Student { get; set; }
        public DbSet<Parent> Parent { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<AcademicPerformance> AcademicPerfomance { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<ClassType> ClassType { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<TypeControl> TypeControl { get; set; }
        public DbSet<StudentsByParents> StudentsByParents { get; set; }
        public DbSet<Homework> Homework { get; set; }
        public DbSet<Timetable> Timetable { get; set; }
        public DbSet<LessonTime> LessonTime { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
