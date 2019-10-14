using AwesomeSauceCompanyLtd.Infrastructure;
using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AwesomeSauceCompanyLtd
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.ModelBinderProviders.Insert(0, new UserModelBinderProvider());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<OutputFormatterSelector, AcceptHeaderOutputFormatterSelector>();

            services.AddSingleton<IUsers, Users>();

            services.AddTransient<NameRoutingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IUsers users)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseNameRouting();

            app.UseMvc();
        }
    }
}
