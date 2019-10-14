using EnterpriseEmployeeManagementInc.Services;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseEmployeeManagementInc.Infrastructure
{
    public class ThumbnailGenerator : IHostedService
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IEmployees _employees;
        private FileSystemWatcher _fsw;

        public ThumbnailGenerator(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IEmployees employees)
        {
            _hostingEnvironment = hostingEnvironment;
            _employees = employees;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images\\employees");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            _fsw = new FileSystemWatcher(path, "*.*");
            _fsw.Created += FileCreated;
            _fsw.EnableRaisingEvents = true;
            return Task.CompletedTask;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            var filename = Path.GetFileNameWithoutExtension(e.FullPath);
            if (filename.Contains("-thumb"))
            {
                return;
            }


            var parts = filename.Split('-');
            if (parts.Length < 4)
            {
                return;
            }

            int tenantId;
            if (!int.TryParse(parts[0], out tenantId))
            {
                return;
            }

            int employeeId;
            if (!int.TryParse(parts[1], out employeeId))
            {
                return;
            }

            var employee = _employees.WithId(tenantId, employeeId).GetAwaiter().GetResult();
            if (employee == null)
            {
                return;
            }

            try
            {
                using (Image image = Image.Load(e.FullPath))
                {
                    image.Mutate(x => x
                         .Resize(new ResizeOptions
                         {
                             Mode = ResizeMode.Max,
                             Size = new SixLabors.Primitives.Size(150, 150)
                         }));


                    var thumbnailFilename = filename + "-thumb" + Path.GetExtension(e.FullPath);
                    var thumbnailUrl = "images\\employees\\" + thumbnailFilename;
                    var thumbnailPath = Path.Combine(_hostingEnvironment.WebRootPath, thumbnailUrl);

                    if (File.Exists(thumbnailPath))
                    {
                        File.Delete(thumbnailPath);
                    }

                    image.Save(thumbnailPath);

                    employee.Thumbnail = thumbnailFilename;
                }
            }
            catch (IOException)
            {
                // Ugly hack...
                FileCreated(sender, e);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _fsw.Created -= FileCreated;
            _fsw.EnableRaisingEvents = false;
            _fsw.Dispose();
            return Task.CompletedTask;
        }
    }
}
