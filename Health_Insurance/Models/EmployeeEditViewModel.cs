// Models/EmployeeEditViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Health_Insurance.Models // Ensure this namespace is correct
{
    public class EmployeeEditViewModel
    {
        public int EmployeeId { get; set; } // Needed to identify the employee being edited

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        public string Address { get; set; }

        [StringLength(50)]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Organization is required.")]
        [Display(Name = "Organization")] // Display name for the label
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        // Password for updating. NOT [Required] for edit scenarios.
        // It's recommended to have a separate view model for password changes,
        // but for now, we make it optional here.
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters if provided.")]
        [Display(Name = "Password (Leave blank to keep current)")]
        public string Password { get; set; } // This will be the plain-text password from the form
    }
}
