// Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Health_Insurance.Models // Ensure this namespace is correct
{
    // ViewModel for handling login form data
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } // Ensure 'Username' is spelled exactly like this

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)] // Suggests a password input type in HTML
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public string Password { get; set; } // Ensure 'Password' is spelled exactly like this

        // New property to indicate the type of login attempt (e.g., "Admin", "Employee")
        [Required(ErrorMessage = "Login type is required.")]
        public string LoginType { get; set; } // Ensure 'LoginType' is spelled exactly like this
    }
}


