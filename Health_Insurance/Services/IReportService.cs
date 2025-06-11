// Services/IReportService.cs
using Health_Insurance.Models; // Ensure this namespace is correct
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Health_Insurance.Services // Ensure this namespace is correct
{
    // Interface defining the contract for the Reporting Service.
    public interface IReportService
    {
        // Method to get data for an Employee Report.
        // Returns a list of Employee objects, potentially with related data.
        Task<List<Employee>> GenerateEmployeeReportAsync();

        // Method to get data for a Policy Report.
        // Returns a list of Policy objects, potentially with related data.
        Task<List<Policy>> GeneratePolicyReportAsync();

        // Method to get data for a list of all enrollments
        Task<List<Enrollment>> GenerateAllEnrollmentsReportAsync();

        // Method to get data for a list of all claims
        Task<List<Claim>> GenerateAllClaimsReportAsync();

        // Method to export report data to different formats (e.g., CSV, PDF, Excel).
        // The return type can be a byte array for file content, or stream.
        // For simplicity, we'll start with a basic string for CSV, then potentially byte array.
        // Parameters: reportType (e.g., "Employee", "Policy"), format (e.g., "CSV", "Excel").
        Task<byte[]> ExportReportAsync(string reportType, string format); // Return byte array for file content
    }
}
