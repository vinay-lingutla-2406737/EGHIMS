// Models/HR.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Health_Insurance.Models
{
    public class HR
    {
        [Key]
        public int HRId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces.")] // NEW VALIDATION
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [StringLength(15)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number can only contain numbers.")] // NEW VALIDATION
        public string Phone { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password hash is required.")]
        [StringLength(256)]
        public string PasswordHash { get; set; }
    }
}