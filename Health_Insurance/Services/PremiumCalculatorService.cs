// Services/PremiumCalculatorService.cs
using Health_Insurance.Data; // Ensure namespace is correct for your DbContext
using Health_Insurance.Models; // Ensure namespace is correct for your Models
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Health_Insurance.Services // Ensure this namespace is correct for your Services folder
{
    // Implementation of the Premium Calculator Service
    public class PremiumCalculatorService : IPremiumCalculatorService // Explicitly implement the interface
    {
        private readonly ApplicationDbContext _context; // DbContext for database interaction

        // Constructor: Inject the ApplicationDbContext
        public PremiumCalculatorService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to calculate the premium for a specific employee and policy
        public async Task<decimal> CalculatePremiumAsync(int employeeId, int policyId)
        {
            // In a real-world scenario, premium calculation is complex.
            // It might depend on:
            // - Policy's base premium (Policy.PremiumAmount)
            // - Employee's age (requires Employee model to have DateOfBirth)
            // - Employee's designation or salary band
            // - Number of dependents (requires a Dependents table/model)
            // - Organization-specific rates
            // - Policy type (Individual vs Family)
            // - Health factors (if applicable)

            // For this basic implementation, let's start by using the Policy's base premium.
            // We'll fetch the Policy to get its PremiumAmount.

            var policy = await _context.Policies.FindAsync(policyId);

            if (policy == null)
            {
                // Policy not found, cannot calculate premium
                // In a real app, you might throw an exception or return a specific error indicator
                return 0; // Return 0 or a specific error value for now
            }

            // Basic Calculation: Return the policy's base premium amount
            decimal calculatedPremium = policy.PremiumAmount;

            // --- Add more complex calculation logic here based on business rules ---
            // Example: Adjust premium based on employee's designation (requires fetching employee)
            // var employee = await _context.Employees.FindAsync(employeeId);
            // if (employee != null)
            // {
            //     if (employee.Designation == "Senior Manager")
            //     {
            //         calculatedPremium *= 1.1m; // 10% increase for Senior Managers
            //     }
            //     // Add other rules based on designation, age, etc.
            // }
            // --- End complex calculation logic ---


            return calculatedPremium;
        }

        // You would add other calculation methods here if needed
    }
}

