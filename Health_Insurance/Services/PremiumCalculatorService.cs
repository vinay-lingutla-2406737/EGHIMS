// Services/PremiumCalculatorService.cs
using Health_Insurance.Data; // Ensure namespace is correct for your DbContext
using Health_Insurance.Models; // Ensure namespace is correct for your Models
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System; // Required for ArgumentException or general Exceptions

namespace Health_Insurance.Services // Ensure this namespace is correct for your Services folder
{
    // Interface for the Premium Calculator Service
   

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
            // 1. Fetch the Policy to get its base premium amount
            var policy = await _context.Policies.FindAsync(policyId);

            if (policy == null)
            {
                // Policy not found, cannot calculate premium
                // In a real app, you might throw an exception, log an error, or return a specific error indicator
                throw new ArgumentException($"Policy with ID {policyId} not found.");
            }

            // 2. Fetch the Employee to get their Designation
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                // Employee not found, cannot calculate premium
                throw new ArgumentException($"Employee with ID {employeeId} not found.");
            }

            // Start with the policy's base premium amount
            decimal calculatedPremium = policy.CoverageAmount*100;

            // --- Apply adjustments based on Employee's Designation ---
            // This is a simplified example. You might have a lookup table for these multipliers
            // or more complex rules. Ensure Designations are consistent (e.g., all uppercase or lowercase).

            if (!string.IsNullOrEmpty(employee.Designation))
            {
                switch (employee.Designation.ToUpperInvariant()) // Use ToUpperInvariant() for robust comparison
                {
                    case "CEO":
                        calculatedPremium *= 2.5m; // Example: CEO pays 2.5x the base premium
                        break;
                    case "DIRECTOR":
                        calculatedPremium *= 2.0m; // Example: Director pays 2.0x
                        break;
                    case "MANAGER":
                        calculatedPremium *= 1.5m; // Example: Manager pays 1.5x
                        break;
                    case "SENIOR ASSOCIATE":
                        calculatedPremium *= 1.2m; // Example: Senior Associate pays 1.2x
                        break;
                    case "ASSOCIATE":
                        calculatedPremium *= 1.0m;
                        break;
                    case "INTERN":
                        calculatedPremium *= 1.0m;
                        break;
                    // Add more designations and their respective premium adjustments
                    default:
                        // No specific adjustment for unknown or unlisted designations
                        calculatedPremium *= 1.0m;
                        break;
                }
            }

            // --- Consider other factors if needed (without using age directly) ---
            // Example: Policy Type (Individual vs Family) - if Policy model has this
            if (policy.PolicyType?.ToUpperInvariant() == "FAMILY FLOATER")
            {
                calculatedPremium *= 1.8m; // Additional multiplier for family policies
            }

            return calculatedPremium;
        }

        // You would add other calculation methods here if needed
    }
}