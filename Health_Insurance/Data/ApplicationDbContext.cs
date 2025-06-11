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


        // Optional: Configure model properties using the Fluent API
        // This is an alternative to Data Annotations (like [Required], [StringLength])
        // You can configure relationships, data types, etc. here.
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Example: Configure the relationship between Employee and Enrollment
        //    modelBuilder.Entity<Enrollment>()
        //        .HasOne(e => e.Employee) // An Enrollment has one Employee
        //        .WithMany() // An Employee can have many Enrollments (or specify the navigation property in Employee model)
        //        .HasForeignKey(e => e.EmployeeId); // The foreign key property

        //    // Example: Configure the relationship between Policy and Enrollment
        //    modelBuilder.Entity<Enrollment>()
        //       .HasOne(e => e.Policy) // An Enrollment has one Policy
        //       .WithMany() // A Policy can have many Enrollments (or specify the navigation property in Policy model)
        //       .HasForeignKey(e => e.PolicyId); // The foreign key property

        //    // Configure PolicyType and Status to be stored as strings
        //    modelBuilder.Entity<Policy>()
        //        .Property(p => p.PolicyType)
        //        .HasConversion<string>(); // Optional, but good practice if you were using enums

        //    modelBuilder.Entity<Enrollment>()
        //        .Property(e => e.Status)
        //        .HasConversion<string>(); // Optional, but good practice if you were using enums

        //    // Configure ClaimStatus to be stored as a string
        //    modelBuilder.Entity<Claim>()
        //        .Property(c => c.ClaimStatus)
        //        .HasConversion<string>(); // Optional, but good practice if you were using enums

        //    // Configure the relationship between Enrollment and Claim
        //    modelBuilder.Entity<Claim>()
        //        .HasOne(c => c.Enrollment) // A Claim has one Enrollment
        //        .WithMany() // An Enrollment can have many Claims (or specify the navigation property in Enrollment model)
        //        .HasForeignKey(c => c.EnrollmentId); // The foreign key property


        //    // Base class OnModelCreating call
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}