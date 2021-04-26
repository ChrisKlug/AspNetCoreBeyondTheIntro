using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RequestDiagnostics
{
    public class DiagnosticMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IDiagnosticsLog _log;

        public DiagnosticMiddleware(RequestDelegate next, IWebHostEnvironment env, IDiagnosticsLog log)
        {
            _next = next;
            _env = env;
            _log = log;
        }

        public async Task Invoke(HttpContext ctx)
        {
            _log.Log($"{DateTime.Now.ToString("yyy-MM-dd HH:mm:ss")}::{ctx.Request.Method} | {ctx.Request.Path}");

            var path = ctx.Request.Path;

            if (path == "/diagnostics" && _env.IsDevelopment())
            {
                var sb = new StringBuilder();

                var log = _log.GetContent();
                for (int i = log.Length - 1; i >= 0; i--)
                {
                    sb.Append(log[i] + "\r\n");
                }

                ctx.Response.ContentType = "text/plain";
                await ctx.Response.WriteAsync(sb.ToString());
                return;
            }

            await _next(ctx);
        }
    }
}