using AISqlAssistant.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AISqlAssistant.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Designation> Designations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Country)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CountryID);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DesignationID);
        }
    }
}
