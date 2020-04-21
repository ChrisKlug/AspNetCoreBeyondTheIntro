using AwesomeSauceCompanyLtd.Infrastructure;
using AwesomeSauceCompanyLtd.Middlewares;
using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AwesomeSauceCompanyLtd
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.ModelBinderProviders.Insert(0, new UserModelBinderProvider());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<OutputFormatterSelector, AcceptHeaderOutputFormatterSelector>();

            services.AddSingleton<IUsers, Users>();

            services.AddTransient<NameRoutingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseNameRouting();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
