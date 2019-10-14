using AwesomeSauceCompanyLtd.Models;
using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsers _users;

        public HomeController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = new HomePageModel
            {
                User1 = await _users.WhereIdIs(1),
                User2 = await _users.WhereIdIs(2),
                User3 = await _users.WhereIdIs(3)
            };
            return View(model);
        }
    }
}