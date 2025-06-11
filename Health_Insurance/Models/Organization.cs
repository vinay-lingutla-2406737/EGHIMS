// Models/Organization.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Health_Insurance.Models // Ensure this namespace is correct based on your project name
{
    // Represents an Organization entity, mapping to the Organization table in the database.
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        [Required]
        [StringLength(100)]
        public string OrganizationName { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string ContactEmail { get; set; }

        // Navigation property for Employees belonging to this Organization
        // public virtual ICollection<Employee> Employees { get; set; }
    }
}

