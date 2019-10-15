using EnterpriseEmployeeManagementInc.Models;
using EnterpriseEmployeeManagementInc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace EnterpriseEmployeeManagementInc.Controllers
{
    [Route("employees")]
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployees _employees;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmployeeController(IEmployees employees, IHostingEnvironment hostingEnvironment)
        {
            _employees = employees;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("add")]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost("add")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(AddEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _employees.Add(model.FirstName, model.LastName, model.Title);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("{employeeId}/profilepicture/upload")]
        public async Task<IActionResult> UploadProfilePicture(int employeeId)
        {
            if (employeeId < 1)
            {
                return BadRequest();
            }

            var employee = await _employees.WithId(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost("{employeeId}/profilepicture/upload")]
        public async Task<IActionResult> UploadProfilePicture(int employeeId, IFormFile picture)
        {
            if (picture == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var employee = await _employees.WithId(employeeId);

            var fileName = $"{employee.TenantId}-{employee.Id}-{employee.FirstName}-{employee.LastName}" + Path.GetExtension(picture.FileName);
            var file = new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, "images\\employees", fileName));
            if (file.Exists)
            {
                file.Delete();
            }
            using (var fs = file.Create())
            {
                await picture.CopyToAsync(fs);
            }
            employee.Picture = fileName;
            return RedirectToAction("Index", "Home");
        }

    }
}