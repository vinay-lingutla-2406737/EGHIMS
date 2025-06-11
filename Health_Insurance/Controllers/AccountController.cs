// Controllers/AccountController.cs
using Health_Insurance.Data;
using Health_Insurance.Models;
using Health_Insurance.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Health_Insurance.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        //private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public AccountController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Employee"); // Admin goes to Employee management
                }
                else if (User.IsInRole("Employee"))
                {
                    var employeeIdClaim = User.FindFirst("EmployeeId");
                    if (employeeIdClaim != null && int.TryParse(employeeIdClaim.Value, out int employeeId))
                    {
                        return RedirectToAction("EnrolledPolicies", "Enrollment", new { employeeId = employeeId });
                    }
                    return RedirectToAction("Index", "Home");
                }
                // --- NEW: Redirect HR users ---
                else if (User.IsInRole("HR"))
                {
                    // HR personnel might also manage employees, so redirect them to Employee/Index
                    return RedirectToAction("Index", "Employee");
                }
                // --- END NEW ---
                return RedirectToAction("Index", "Home"); // Fallback
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.AuthenticateUserAsync(model.Username, model.Password, model.LoginType);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password, or incorrect login type selected.");
                return View(model);
            }

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, model.Username),
            };

            string userRole = string.Empty;
            int userId = 0; // To store AdminId, EmployeeId, or HRId

            if (user is Admin adminUser)
            {
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, "Admin"));
                claims.Add(new System.Security.Claims.Claim("AdminId", adminUser.AdminId.ToString()));
                userRole = "Admin";
                userId = adminUser.AdminId;
            }
            else if (user is Employee employeeUser)
            {
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, "Employee"));
                claims.Add(new System.Security.Claims.Claim("EmployeeId", employeeUser.EmployeeId.ToString()));
                userRole = "Employee";
                userId = employeeUser.EmployeeId;
            }
            // --- NEW: Handle HR claims and role ---
            else if (user is HR hrUser)
            {
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, "HR"));
                claims.Add(new System.Security.Claims.Claim("HRId", hrUser.HRId.ToString())); // Custom claim for HRId
                userRole = "HR";
                userId = hrUser.HRId;
            }
            // --- END NEW ---

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = System.DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect based on role after successful login
            if (userRole == "Admin")
            {
                return RedirectToAction("Index", "Employee");
            }
            else if (userRole == "Employee")
            {
                return RedirectToAction("EnrolledPolicies", "Enrollment", new { employeeId = userId });
            }
            // --- NEW: Redirect HR personnel after login ---
            else if (userRole == "HR")
            {
                return RedirectToAction("Index", "Employee"); // HR also goes to Employee management initially
            }
            // --- END NEW ---
            return RedirectToAction("Index", "Home"); // Fallback redirect
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }


        [Authorize(Roles = "Admin")] // Only Admins can access this dashboard
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard"; // Set title for the layout
            return View(); // This will look for Views/Account/Dashboard.cshtml
        }




        
        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/policies")]
        public async Task<JsonResult> GetPoliciesApi() // Marked as async Task<JsonResult>
        {
            var policies = await _context.Policies.ToListAsync();

            // Select only the properties needed by the JavaScript frontend
            // Ensure property names match what the JS expects (e.g., 'id', 'name', 'premium', 'enrolled')
            var policiesForFrontend = policies.Select(p => new
            {
                id = p.PolicyId, // Assuming PolicyId is your primary key
                name = p.PolicyName, // Assuming PolicyName is the property for policy name
                type = p.PolicyType, // Assuming PolicyType property
                
                coverage = $"₹{p.CoverageAmount.ToString("N0")}", // Format currency in C# if needed, or handle in JS
                premium = p.PremiumAmount.ToString(), // Send as string to match JS's parsing logic (remove '₹')
               
                
            }).ToList();

            return Json(policiesForFrontend);
        
        }


 // API Endpoint for Employees Data - Fetches from DB based on provided model
        [HttpGet("/api/admin/employees")]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetEmployeesApi()
        {
            // Include Organization for potentially displaying organization name
            var employees = await _context.Employees
                                        .Include(e => e.Organization) // Assuming navigation property 'Organization' exists
                                        .ToListAsync();

            var employeesForFrontend = employees.Select(e => new
            {
                id = e.EmployeeId,
                name = e.Name,
                email = e.Email ?? "N/A",      // Email can be null
                phone = e.Phone ?? "N/A",      // Phone can be null
                address = e.Address ?? "N/A",  // Address can be null
                department = e.Designation ?? "N/A", // 'Designation' from DB maps to 'department' in frontend
                organizationId = e.OrganizationId,
                organizationName = e.Organization != null ? e.Organization.OrganizationName : "N/A", // Get Org Name
                username = e.Username ?? "N/A", // Username can be null (though [Required] on model)
                // 'policyStatus' and 'dependents' are not directly available in Employee model.
                // These would need to be fetched/calculated from 'Enrollment' and 'Dependent' tables respectively.
                // For now, they are not included in the projection.
            }).ToList();

            return Json(employeesForFrontend);
        }

        // API Endpoint for Claims Data - Fetches from DB based on provided model
        [HttpGet("/api/admin/claims")]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetClaimsApi()
        {
            // Include Enrollment and then Employee and Policy to get related data
            var claims = await _context.Claims
                                       .Include(c => c.Enrollment)
                                           .ThenInclude(e => e.Employee) // To get Employee Name
                                       .Include(c => c.Enrollment)
                                           .ThenInclude(e => e.Policy) // To get Policy Type
                                       .ToListAsync();

            var claimsForFrontend = claims.Select(c => new
            {
                id = c.ClaimId,
                employeeName = c.Enrollment?.Employee?.Name ?? "N/A", // Safely get employee name
                policyType = c.Enrollment?.Policy?.PolicyType ?? "N/A", // Safely get policy type from Policy model
                claimDate = c.ClaimDate.ToString("yyyy-MM-dd"), // Format date for JS
                requestedAmount = c.ClaimAmount, // ClaimAmount maps to requestedAmount
                // Your Claim model does NOT have an 'ApprovedAmount' property.
                // This is a placeholder logic. You might need to add 'ApprovedAmount' to your Claim model
                // and database if you need to store this explicitly.
                approvedAmount = (c.ClaimStatus?.ToUpper() == "APPROVED" ? c.ClaimAmount : 0),
                status = c.ClaimStatus ?? "N/A" // ClaimStatus maps to status
            }).ToList();

            return Json(claimsForFrontend);
        }

        // API Endpoint for Claims Trend Data - Fetches and processes from DB based on provided model
        [HttpGet("/api/admin/claims-trend")]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetClaimsTrendApi()
        {
            // Fetch claims from the last 6 months (adjust as needed)
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var claims = await _context.Claims
                                       .Where(c => c.ClaimDate >= sixMonthsAgo)
                                       .ToListAsync();

            // Group claims by month and status
            var claimsTrend = claims
                .GroupBy(c => new { c.ClaimDate.Year, c.ClaimDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"), // e.g., "Jan", "Feb"
                    approved = g.Count(c => c.ClaimStatus?.ToUpper() == "APPROVED"),
                    submitted = g.Count(c => c.ClaimStatus?.ToUpper() == "SUBMITTED"),
                    rejected = g.Count(c => c.ClaimStatus?.ToUpper() == "REJECTED")
                })
                .ToList();

            // Ensure we have data for the last 6 months even if no claims occurred
            var currentMonth = DateTime.UtcNow;
            var fullTrend = new List<object>();
            for (int i = 5; i >= 0; i--) // Go back 5 months from current, plus current (total 6)
            {
                var monthDate = currentMonth.AddMonths(-i);
                var monthName = monthDate.ToString("MMM");
                var existingMonthData = claimsTrend.FirstOrDefault(t => (string)t.GetType().GetProperty("month").GetValue(t) == monthName);

                if (existingMonthData == null)
                {
                    fullTrend.Add(new { month = monthName, approved = 0, submitted = 0, rejected = 0 });
                }
                else
                {
                    fullTrend.Add(existingMonthData);
                }
            }
            return Json(fullTrend);
        }

        // API Endpoint for Organizations Data - Fetches from DB based on provided model
        [HttpGet("/api/admin/organizations")]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetOrganizationsApi()
        {
            var organizations = await _context.Organizations.ToListAsync();

            var organizationsForFrontend = organizations.Select(o => new
            {
                id = o.OrganizationId,
                name = o.OrganizationName,
                contactPerson = o.ContactPerson ?? "N/A", // Can be null
                contactEmail = o.ContactEmail ?? "N/A",   // Can be null
                // 'employeesCount' and 'policiesEnrolled' are not direct properties.
                // These would require additional queries/joins to calculate from Employees and Enrollments.
                // For now, they are excluded from the projection.
            }).ToList();

            return Json(organizationsForFrontend);
        }
    }
}

