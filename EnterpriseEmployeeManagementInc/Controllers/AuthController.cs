using System.Threading.Tasks;
using EnterpriseEmployeeManagementInc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseEmployeeManagementInc.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUsers _users;

        public AuthController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]string username, [FromForm]string password, string returnUrl = "/")
        {
            var user = await _users.Authenticate(username, password);

            if (user == null)
            {
                TempData["InvalidCredentials"] = true;
                return RedirectToAction("Login", new { returnUrl });
            }

            await HttpContext.SignInAsync(user.AsPrincipal(CookieAuthenticationDefaults.AuthenticationScheme));

            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            return Redirect(returnUrl);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}