using AnsibeProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AnsibeProject.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions options) : base(options) { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Professor> Professors { get; set;}
        public DbSet<Contract> Contracts { get; set; }

        public DbSet<Section> Sections { get; set; }
        public DbSet<Ansibe> Ansibes { get; set; }
    }
}
