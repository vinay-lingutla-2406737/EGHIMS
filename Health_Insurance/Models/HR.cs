// Models/HR.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Health_Insurance.Models // Ensure this namespace is correct
{
    // Represents an HR personnel entity, with login capabilities.
    public class HR
    {
        [Key] // Specifies this property is the primary key
        public int HRId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        [StringLength(256)] // Store the hashed password
        public string PasswordHash { get; set; }
    }
}
