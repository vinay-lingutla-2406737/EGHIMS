// Controllers/EmployeeController.cs
using Health_Insurance.Data;
using Health_Insurance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization; // Add this using statement
using BCrypt.Net;

namespace Health_Insurance.Controllers
{
    // Restrict all actions in this controller to users with the "Admin" OR "HR" role.
    [Authorize(Roles = "Admin,HR")] // --- MODIFIED: Added "HR" role ---
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Employee/Index
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.Include(e => e.Organization).ToListAsync();
            return View(employees);
        }

        // GET: /Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: /Employee/Create
        public IActionResult Create()
        {
            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName");
            return View();
        }

        // POST: /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address,Designation,OrganizationId,Username,PasswordHash")] Employee employee)
        {
            // Hash the password before saving a new employee
            if (!string.IsNullOrEmpty(employee.PasswordHash))
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);
            }
            else
            {
                // If PasswordHash is empty, and it's required for new user creation,
                // add a model error.
                ModelState.AddModelError(nameof(employee.PasswordHash), "Password is required for new employees.");
            }

            // Check if OrganizationId is valid before proceeding
            if (employee.OrganizationId <= 0)
            {
                ModelState.AddModelError(nameof(employee.OrganizationId), "Please select a valid organization.");
            }

            if (ModelState.IsValid)
            {
                // Manually retrieve and assign the Organization navigation property
                var organization = await _context.Organizations.FindAsync(employee.OrganizationId);
                if (organization == null)
                {
                    ModelState.AddModelError(nameof(employee.OrganizationId), "Selected organization not found.");
                    ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName", employee.OrganizationId);
                    return View(employee);
                }
                employee.Organization = organization;

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName", employee.OrganizationId);
            return View(employee);
        }

        // GET: /Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                Designation = employee.Designation,
                OrganizationId = employee.OrganizationId,
                Username = employee.Username,
                Password = null
            };

            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName", employee.OrganizationId);
            return View(viewModel);
        }

        // POST: /Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel viewModel)
        {
            if (id != viewModel.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.ContainsKey(nameof(viewModel.Password)) && string.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.Remove(nameof(viewModel.Password));
            }

            if (ModelState.IsValid)
            {
                var employeeToUpdate = await _context.Employees.FindAsync(id);

                if (employeeToUpdate == null)
                {
                    return NotFound();
                }

                employeeToUpdate.Name = viewModel.Name;
                employeeToUpdate.Email = viewModel.Email;
                employeeToUpdate.Phone = viewModel.Phone;
                employeeToUpdate.Address = viewModel.Address;
                employeeToUpdate.Designation = viewModel.Designation;
                employeeToUpdate.OrganizationId = viewModel.OrganizationId;
                employeeToUpdate.Username = viewModel.Username;

                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    employeeToUpdate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);
                }

                try
                {
                    _context.Update(employeeToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(viewModel.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "OrganizationId", "OrganizationName", viewModel.OrganizationId);
            return View(viewModel);
        }

        // GET: /Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
