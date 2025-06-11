// Services/IPremiumCalculatorService.cs
using Health_Insurance.Models; // Ensure namespace is correct for your Models
using System.Threading.Tasks;

namespace Health_Insurance.Services // Ensure this namespace is correct for your Services folder
{
    // Interface defining the contract for the Premium Calculator Service
    public interface IPremiumCalculatorService
    {
        // Method to calculate the premium for a specific employee and policy
        // This method will likely take Employee and Policy objects or IDs as input
        // and return the calculated premium amount.
        Task<decimal> CalculatePremiumAsync(int employeeId, int policyId);

        // You might add other methods here if calculation needs variations
        // e.g., CalculatePremiumWithDependents, EstimatePremium
    }
}

