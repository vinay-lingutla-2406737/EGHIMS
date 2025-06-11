// Services/IClaimService.cs
using Health_Insurance.Models; // Ensure namespace is correct for your Models
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Health_Insurance.Services // Ensure this namespace is correct for your Services folder
{
    // Interface defining the contract for the Claim Service
    public interface IClaimService
    {
        // Method to submit a new claim
        // Takes a Claim object and potentially performs validation
        Task<bool> SubmitClaimAsync(Claim claim);

        // Method to get details of a specific claim by ID
        Task<Claim> GetClaimDetailsAsync(int claimId);

        // Method to update the status of a claim
        // Takes Claim ID and the new status
        Task<bool> UpdateClaimStatusAsync(int claimId, string newStatus);

        // Method to list all claims (or claims based on criteria)
        Task<IEnumerable<Claim>> ListAllClaimsAsync();

        // You might add other methods here, e.g., ValidateClaim, ProcessApproval
    }
}

