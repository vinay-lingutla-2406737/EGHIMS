// Data/ApplicationDbContext.cs
using Health_Insurance.Models; // Make sure namespace is correct for your Models
using Microsoft.EntityFrameworkCore;

namespace Health_Insurance.Data // Ensure this namespace is correct for your Data folder
{
    // DbContext is the main class that coordinates Entity Framework functionality
    // for a given data model. It MUST inherit from DbContext.
    public class ApplicationDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions, typically configured in Program.cs/Startup.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties represent collections of entities that can be queried from the database.
        // These will map to your database tables.
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        // Add DbSet for the Claim model
        public DbSet<Claim> Claims { get; set; } // Add this line
        // --- Add DbSets for Authentication Models ---
        public DbSet<Admin> Admins { get; set; }
        // --- End DbSets for Authentication Models ---
        public DbSet<HR> HRs { get; set; }

    }
}