using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace RequestDiagnostics
{
    public interface IDiagnosticsLog
    {
        void Log(string data);
        string[] GetContent();
    }

    public class DiagnosticsLog : IDiagnosticsLog
    {
        const string LogName = "diagnostics.log";
        private List<string> _entries;

        public DiagnosticsLog(IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            EnsureEntries(env);

            var file = env.ContentRootFileProvider.GetFileInfo(LogName);
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                File.WriteAllText(file.PhysicalPath, JsonConvert.SerializeObject(_entries));
            });
        }

        public void Log(string data)
        {
            _entries.Add(data);
        }

        public string[] GetContent()
        {
            return _entries.ToArray();
        }

        private void EnsureEntries(IHostingEnvironment env)
        {
            var file = env.ContentRootFileProvider.GetFileInfo(LogName);
            if (!file.Exists)
            {
                _entries = new List<string>();
                return;
            }

            _entries = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(file.PhysicalPath));
        }
    }
}