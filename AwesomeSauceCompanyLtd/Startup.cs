using AwesomeSauceCompanyLtd.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AwesomeSauceCompanyLtd.Infrastructure;

namespace AwesomeSauceCompanyLtd
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options => {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.ModelBinderProviders.Insert(0, new UserModelBinderProvider());
            });

            services.AddSingleton<OutputFormatterSelector, AcceptHeaderOutputFormatterSelector>();

            services.AddSingleton<IUsers, Users>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUsers users, IDistributedCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.Use(async (ctx, next) =>
            {
                var item = await cache.GetAsync(ctx.Request.Path);
                if (item != null)
                {
                    await ctx.Response.Body.WriteAsync(item, 0, item.Length);
                    return;
                }

                await next();

                if (ctx.Items.TryGetValue("addToCache", out var bytes))
                {
                    await cache.SetAsync(ctx.Request.Path,
                                        (byte[])bytes,
                                        new DistributedCacheEntryOptions
                                        {
                                            AbsoluteExpirationRelativeToNow = new System.TimeSpan(0, 10, 0)
                                        });
                }
            });

            app.Use(async (ctx, next) =>
            {
                using (var ms = new MemoryStream())
                {
                    var rs = ctx.Response.Body;
                    ctx.Response.Body = ms;

                    await next();

                    var str = Encoding.UTF8.GetString(ms.ToArray());

                    if (new Regex("/users/[0-9]+").IsMatch(str))
                    {
                        str = (await users.All()).Aggregate(str, (x, user) => x.Replace($"/users/{user.Id}", $"/{user.FirstName.ToLower()}-{user.LastName.ToLower()}"));

                        var bytes = Encoding.UTF8.GetBytes(str);
                        await rs.WriteAsync(bytes, 0, bytes.Length);
                        ctx.Items["addToCache"] = bytes;
                    }
                    else
                    {
                        await rs.WriteAsync(ms.ToArray());
                    }
                }
            });

            app.UseNameRouting();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
