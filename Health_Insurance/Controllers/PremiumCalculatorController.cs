// Controllers/PremiumCalculatorController.cs
using Health_Insurance.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Add this using statement
using System.Security.Claims; // Needed for User.FindFirst
using Health_Insurance.Data; // Needed to fetch Employees for dropdown
using Microsoft.EntityFrameworkCore; // Needed for ToListAsync()
using Microsoft.AspNetCore.Mvc.Rendering; // Needed for SelectList

namespace Health_Insurance.Controllers
{
    // All actions in this controller require authentication.
    [Authorize]
    public class PremiumCalculatorController : Controller
    {
        private readonly IPremiumCalculatorService _premiumCalculatorService;
        private readonly ApplicationDbContext _context; // Inject DbContext to get Employees for dropdown

        public PremiumCalculatorController(IPremiumCalculatorService premiumCalculatorService, ApplicationDbContext context)
        {
            _premiumCalculatorService = premiumCalculatorService;
            _context = context;
        }

        // GET: /PremiumCalculator/CalculatePremium
        public async Task<IActionResult> CalculatePremium()
        {
            if (User.IsInRole("Employee"))
            {
                var loggedInEmployeeId = User.FindFirst("EmployeeId")?.Value;
                if (loggedInEmployeeId != null && int.TryParse(loggedInEmployeeId, out int employeeId))
                {
                    // If an employee is logged in, pre-select their ID and disable the dropdown
                    ViewBag.EmployeeList = new SelectList(new List<Health_Insurance.Models.Employee> { await _context.Employees.FindAsync(employeeId) }, "EmployeeId", "Name", employeeId);
                    ViewBag.IsEmployee = true; // Flag for the view to disable dropdown
                }
                else
                {
                    // Fallback if employee ID not found in claims, though it should be
                    ViewBag.EmployeeList = new SelectList(new List<Health_Insurance.Models.Employee>());
                    ViewBag.IsEmployee = true;
                }
            }
            else // Admin or other roles
            {
                var employees = await _context.Employees.ToListAsync();
                ViewBag.EmployeeList = new SelectList(employees, "EmployeeId", "Name");
                ViewBag.IsEmployee = false;
            }

            // Fetch all policies to populate the dropdown
            ViewBag.PolicyList = new SelectList(await _context.Policies.ToListAsync(), "PolicyId", "PolicyName");

            return View();
        }

        // POST: /PremiumCalculator/CalculatePremium
        [HttpPost]
        // [ValidateAntiForgeryToken] // Uncomment if you add AntiForgeryToken to the AJAX form
        public async Task<IActionResult> CalculatePremium(int employeeId, int policyId)
        {
            // Enforce that an Employee can only calculate premium for their own ID
            if (User.IsInRole("Employee"))
            {
                var loggedInEmployeeId = User.FindFirst("EmployeeId")?.Value;
                if (loggedInEmployeeId == null || !int.TryParse(loggedInEmployeeId, out int actualEmployeeId) || actualEmployeeId != employeeId)
                {
                    return Forbid(); // Attempt by an Employee to calculate for someone else
                }
            }

            var calculatedPremium = await _premiumCalculatorService.CalculatePremiumAsync(employeeId, policyId);
            return Json(new { premium = calculatedPremium });
        }
    }
}

