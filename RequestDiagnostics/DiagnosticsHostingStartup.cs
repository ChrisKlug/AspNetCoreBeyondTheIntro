using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RequestDiagnostics;

[assembly: HostingStartup(typeof(DiagnosticsHostingStartup))]

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