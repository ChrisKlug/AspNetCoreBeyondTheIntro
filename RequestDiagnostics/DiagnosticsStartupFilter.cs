using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace RequestDiagnostics
{
    public class DiagnosticsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Console.WriteLine("Adding generic diagnostics to system. Diagnostics available at /diagnostics");

            return app =>
            {
                app.UseMiddleware<DiagnosticMiddleware>();

                next(app);
            };
        }
    }
}