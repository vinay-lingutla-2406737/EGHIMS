// Models/Enrollment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Health_Insurance.Models // Ensure this namespace is correct based on your project name
{
    // Represents an Enrollment entity, mapping to the Enrollment table in the database.
    public class Enrollment
    {
        [Key] // Specifies this property is the primary key
        public int EnrollmentId { get; set; }

        // Foreign Key to the Employee table
        public int EmployeeId { get; set; }

        // Navigation property to the related Employee
        [ForeignKey("EmployeeId")] // Specifies the foreign key property
        public virtual Employee Employee { get; set; }

        // Foreign Key to the Policy table
        public int PolicyId { get; set; }

        // Navigation property to the related Policy
        [ForeignKey("PolicyId")] // Specifies the foreign key property
        public virtual Policy Policy { get; set; }

        [Required]
        [DataType(DataType.Date)] // Specify the data type for date input
        public DateTime EnrollmentDate { get; set; }

        // Status as described in the document (ENUM in MySQL, using string with validation here)
        [Required]
        [StringLength(20)]
        // You might add validation here or in a Service layer to restrict values to 'ACTIVE', 'CANCELLED'
        public string Status { get; set; }

        // Navigation property for Claims related to this Enrollment (if needed)
        // public virtual ICollection<Claim> Claims { get; set; }
    }
}

