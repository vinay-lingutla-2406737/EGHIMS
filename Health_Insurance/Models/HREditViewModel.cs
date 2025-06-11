// Models/HREditViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Health_Insurance.Models // Ensure this namespace is correct

{

    // ViewModel for handling HR personnel edit form data.

    public class HREditViewModel

    {

        public int HRId { get; set; } // Needed to identify the HR record being edited

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

        // Password field for updating. NOT [Required] for edit scenarios.

        // Will be null if not provided, and handled in the controller.

        [DataType(DataType.Password)]

        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters if provided.")]

        [Display(Name = "Password (Leave blank to keep current)")]

        public string Password { get; set; } // This will be the plain-text password from the form

    }

}

