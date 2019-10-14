using AwesomeSauceCompanyLtd.Services;
using AwesomeSauceCompanyLtd.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsers _users;

        public UserController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(int userId)
        {
            var user = await _users.WhereIdIs(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
