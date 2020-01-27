using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(RequestDiagnostics.DiagnosticsHostingStartup))]

namespace RequestDiagnostics
{
    public class DiagnosticsHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IDiagnosticsLog, DiagnosticsLog>();
                services.AddSingleton<IStartupFilter, DiagnosticsStartupFilter>();
            });
        }
    }
}