// Models/HREditViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Health_Insurance.Models
{
    public class HREditViewModel
    {
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

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters if provided.")]
        [Display(Name = "Password (Leave blank to keep current)")]
        public string Password { get; set; }
    }
}