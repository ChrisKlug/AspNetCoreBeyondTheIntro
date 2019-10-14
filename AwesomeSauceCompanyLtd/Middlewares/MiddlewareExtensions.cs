using AwesomeSauceCompanyLtd.Infrastructure;

namespace Microsoft.AspNetCore.Builder
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseNameRouting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<NameRoutingMiddleware>();
        }
    }
}
