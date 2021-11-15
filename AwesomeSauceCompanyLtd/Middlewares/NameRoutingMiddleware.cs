using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Middlewares
{
    public class NameRoutingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IUsers users;

        public NameRoutingMiddleware(RequestDelegate next, IUsers users)
        {
            this.next = next;
            this.users = users;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path.Length > 1 && path.IndexOf('/', 1) < 0)
            {
                // Single segment path
                var user = await users.WithName(path.Substring(1).Replace("-", " "));
                if (user != null)
                {
                    context.Request.Path = "/users/" + user.Id;
                }
            }

            await next(context);

            context.Request.Path = path;
        }
    }
}