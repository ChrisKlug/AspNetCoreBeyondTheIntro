using EnterpriseEmployeeManagementInc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EnterpriseEmployeeManagementInc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployees _employees;

        public HomeController(IEmployees employees)
        {
            _employees = employees;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _employees.All(User.TenantId()));
        }
    }
}