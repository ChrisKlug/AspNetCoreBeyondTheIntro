using System.Threading.Tasks;
using EnterpriseEmployeeManagementInc.Services.Entities;

namespace EnterpriseEmployeeManagementInc.Services
{
    public interface IEmployees
    {
        Task<Employee[]> All();
        Task<Employee> WithId(int employeeId);
        Task<Employee> WithId(int tenantId, int employeeId);
        Task<Employee> Add(string firstName, string lastName, string title);
    }
}