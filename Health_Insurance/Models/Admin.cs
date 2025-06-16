// Models/Admin.cs
using System.ComponentModel.DataAnnotations;

namespace Health_Insurance.Models // Ensure this namespace is correct based on your project name
{
    // Represents an Admin user for login purposes sssss
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)] // Store hashed passwords
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Admin's full name
    }
}