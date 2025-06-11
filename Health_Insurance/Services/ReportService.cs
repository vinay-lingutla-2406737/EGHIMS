// Services/ReportService.cs
using Health_Insurance.Data; // Ensure this namespace is correct for your DbContext
using Health_Insurance.Models; // Ensure this namespace is correct for your Models
using Microsoft.EntityFrameworkCore; // For ToListAsync, Include
using System.Collections.Generic;
using System.IO; // For StringWriter
using System.Text; // For StringBuilder
using System.Threading.Tasks;
using System; // For ArgumentException, NotSupportedException

namespace Health_Insurance.Services // Ensure this namespace is correct
{
    // Implementation of the Reporting Service.
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context; // Inject your DbContext

        // Constructor to inject ApplicationDbContext.
        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all employees for the Employee Report.
        public async Task<List<Employee>> GenerateEmployeeReportAsync()
        {
            // Include Organization for potentially richer reports
            return await _context.Employees.Include(e => e.Organization).ToListAsync();
        }

        // Retrieves all policies for the Policy Report.
        public async Task<List<Policy>> GeneratePolicyReportAsync()
        {
            return await _context.Policies.ToListAsync();
        }

        // Retrieves all enrollments for the Enrollments Report.
        public async Task<List<Enrollment>> GenerateAllEnrollmentsReportAsync()
        {
            // Include Employee and Policy for richer enrollment reports
            return await _context.Enrollments
                                 .Include(e => e.Employee)
                                 .Include(e => e.Policy)
                                 .ToListAsync();
        }

        // Retrieves all claims for the Claims Report.
        public async Task<List<Claim>> GenerateAllClaimsReportAsync()
        {
            // Include Enrollment, Employee, and Policy for richer claim reports
            return await _context.Claims
                                 .Include(c => c.Enrollment)
                                     .ThenInclude(e => e.Employee) // Chain Include for nested navigation property
                                 .Include(c => c.Enrollment)
                                     .ThenInclude(e => e.Policy) // Chain Include for nested navigation property
                                 .ToListAsync();
        }

        // Placeholder for Exporting reports. Initially, only supports basic CSV.
        // Full Excel/PDF export would require dedicated libraries (e.g., EPPlus, QuestPDF).
        public async Task<byte[]> ExportReportAsync(string reportType, string format)
        {
            // Normalize format input
            format = format?.ToLower();

            if (format == "csv")
            {
                using (var writer = new StringWriter())
                {
                    switch (reportType.ToLower())
                    {
                        case "employee":
                            var employees = await GenerateEmployeeReportAsync();
                            // CSV Header
                            writer.WriteLine("EmployeeId,Name,Email,Phone,Address,Designation,OrganizationId,OrganizationName,Username");
                            // CSV Rows
                            foreach (var emp in employees)
                            {
                                writer.WriteLine($"{emp.EmployeeId},{EscapeCsv(emp.Name)},{EscapeCsv(emp.Email)},{EscapeCsv(emp.Phone)},{EscapeCsv(emp.Address)},{EscapeCsv(emp.Designation)},{emp.OrganizationId},{EscapeCsv(emp.Organization?.OrganizationName)},{EscapeCsv(emp.Username)}");
                            }
                            break;

                        case "policy":
                            var policies = await GeneratePolicyReportAsync();
                            writer.WriteLine("PolicyId,PolicyName,CoverageAmount,PremiumAmount,PolicyType");
                            foreach (var pol in policies)
                            {
                                writer.WriteLine($"{pol.PolicyId},{EscapeCsv(pol.PolicyName)},{pol.CoverageAmount},{pol.PremiumAmount},{EscapeCsv(pol.PolicyType)}");
                            }
                            break;

                        case "enrollment":
                            var enrollments = await GenerateAllEnrollmentsReportAsync();
                            // --- CORRECTED: Removed IsActive from header and row ---
                            writer.WriteLine("EnrollmentId,EmployeeName,PolicyName,EnrollmentDate");
                            foreach (var enr in enrollments)
                            {
                                writer.WriteLine($"{enr.EnrollmentId},{EscapeCsv(enr.Employee?.Name)},{EscapeCsv(enr.Policy?.PolicyName)},{enr.EnrollmentDate:yyyy-MM-dd}");
                            }
                            break;

                        case "claim":
                            var claims = await GenerateAllClaimsReportAsync();
                            writer.WriteLine("ClaimId,EnrollmentId,EmployeeName,PolicyName,ClaimAmount,ClaimDate,ClaimReason,ClaimStatus");
                            foreach (var clm in claims)
                            {
                                writer.WriteLine($"{clm.ClaimId},{clm.EnrollmentId},{EscapeCsv(clm.Enrollment?.Employee?.Name)},{EscapeCsv(clm.Enrollment?.Policy?.PolicyName)},{clm.ClaimAmount},{clm.ClaimDate:yyyy-MM-dd},{EscapeCsv(clm.ClaimReason)},{EscapeCsv(clm.ClaimStatus)}");
                            }
                            break;

                        default:
                            throw new ArgumentException("Invalid report type for CSV export.");
                    }
                    return Encoding.UTF8.GetBytes(writer.ToString());
                }
            }
            else if (format == "excel" || format == "pdf")
            {
                throw new NotSupportedException($"Export to {format.ToUpper()} is not yet implemented.");
            }
            else
            {
                throw new ArgumentException("Unsupported export format.");
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
