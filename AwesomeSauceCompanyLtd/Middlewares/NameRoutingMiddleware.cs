using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Middlewares
{
    public class NameRoutingMiddleware : IMiddleware
    {
        private readonly IUsers _users;

        public NameRoutingMiddleware(IUsers users)
        {
            _users = users;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.Length > 1 && context.Request.Path.Value.IndexOf('/', 1) < 0)
            {
                // Single segment path
                var path = context.Request.Path.Value.Substring(1);
                var user = await _users.WithName(path.Replace("-", " "));
                if (user != null)
                {
                    context.Request.Path = "/users/" + user.Id;
                }

            }
            await next(context);
        }
    }
}