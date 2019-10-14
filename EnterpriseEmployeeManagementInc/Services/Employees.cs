using EnterpriseEmployeeManagementInc.Services.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseEmployeeManagementInc.Services
{
    public class Employees : IEmployees
    {
        private Employee[] _employees;

        public Employees(IHostingEnvironment env)
        {
            _employees = JsonConvert.DeserializeObject<Employee[]>(File.ReadAllText(env.ContentRootFileProvider.GetFileInfo("Data/employees.json").PhysicalPath));
        }

        public Task<Employee[]> All(int tenantId)
        {
            return Task.FromResult(_employees.Where(x => x.TenantId == tenantId).OrderBy(x => x.FirstName).ToArray());
        }

        public Task<Employee> WithId(int tenantId, int employeeId)
        {
            return Task.FromResult(_employees.FirstOrDefault(x => x.TenantId == tenantId && x.Id == employeeId));
        }

        public Task<Employee> Add(int tenantId, string firstName, string lastName, string title)
        {
            var employee = new Employee {
                Id = _employees.Length,
                TenantId = tenantId,
                FirstName = firstName,
                LastName = lastName,
                Title = title
            };
            Array.Resize(ref _employees, _employees.Length + 1);
            _employees[_employees.Length - 1] = employee;
            return Task.FromResult(employee);
        }
    }
}
