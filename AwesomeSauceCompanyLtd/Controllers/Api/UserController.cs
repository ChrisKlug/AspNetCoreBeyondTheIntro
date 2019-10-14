using AwesomeSauceCompanyLtd.Controllers.Api.Models;
using AwesomeSauceCompanyLtd.Infrastructure;
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
        public ActionResult<User> GetUser(User user)
        {
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("{userId}")]
        [AcceptHeader("application/vnd.user")]
        public ActionResult<BasicUser> GetBasicUser(User user)
        {
            if (user == null)
            {
                return NotFound();
            }

            return Ok(BasicUser.Create(user));
        }
    }
}