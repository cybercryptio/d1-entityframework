using ContosoUniversity.Models;
using CyberCrypt.D1.Client;
using CyberCrypt.D1.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class SchoolContext : D1DbContext
    {
        public SchoolContext(Func<ID1Generic> clientFactory, DbContextOptions<SchoolContext> options) : base(clientFactory, options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable(nameof(Course))
                .HasMany(c => c.Instructors)
                .WithMany(i => i.Courses);
            modelBuilder.Entity<Student>().ToTable(nameof(Student));
            modelBuilder.Entity<Instructor>().ToTable(nameof(Instructor));
            modelBuilder.Entity<Student>().Property(x => x.Email).AsSearchable(v => new[] { v });

            base.OnModelCreating(modelBuilder);
        }
    }
}