using Microsoft.EntityFrameworkCore;
using ProjectCalendar.Models;

namespace ProjectCalendar.DB
{
    public class ProjectContext : DbContext 
    {
        public DbSet<Dates> Dates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Person> Persons { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options):base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dates>().HasKey(e => new { e.Date, e.ProjectId, e.PersonId });
        }

    }

}
