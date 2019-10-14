using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IUsers _users;

        public UserController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Index(int userId)
        {
            var user = await _users.WhereIdIs(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}