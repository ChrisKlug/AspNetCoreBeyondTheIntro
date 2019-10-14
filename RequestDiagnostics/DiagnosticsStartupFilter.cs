using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace RequestDiagnostics
{
    public class DiagnosticsStartupFilter : IStartupFilter
    {
        public DiagnosticsStartupFilter()
        {
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Console.WriteLine("Adding generic diagnostics to system. Dagnostics available at /diagnostics");

            return app =>
            {
                app.UseMiddleware<DiagnosticMiddleware>();

                next(app);
            };
        }
    }
}