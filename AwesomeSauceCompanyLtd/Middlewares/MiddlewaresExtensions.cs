using AwesomeSauceCompanyLtd.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class MiddlewaresExtensions
    {
        public static IApplicationBuilder UseNameRouting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<NameRoutingMiddleware>();
        }
    }
}
