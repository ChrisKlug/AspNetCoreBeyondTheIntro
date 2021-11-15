using EnterpriseEmployeeManagementInc.Services.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Employees(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _employees = JsonConvert.DeserializeObject<Employee[]>(File.ReadAllText(env.ContentRootFileProvider.GetFileInfo("Data/employees.json").PhysicalPath));
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<Employee[]> All()
        {
            return Task.FromResult(_employees.Where(x => x.TenantId == _httpContextAccessor.HttpContext.User.TenantId()).OrderBy(x => x.FirstName).ToArray());
        }

        public Task<Employee> WithId(int employeeId)
        {
            return WithId(_httpContextAccessor.HttpContext.User.TenantId(), employeeId);
        }

        public Task<Employee> WithId(int tenantId, int employeeId)
        {
            return Task.FromResult(_employees.FirstOrDefault(x => x.TenantId == _httpContextAccessor.HttpContext.User.TenantId() && x.Id == employeeId));
        }

        public Task<Employee> Add(string firstName, string lastName, string title)
        {
            var employee = new Employee
            {
                Id = _employees.Length,
                TenantId = _httpContextAccessor.HttpContext.User.TenantId(),
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
