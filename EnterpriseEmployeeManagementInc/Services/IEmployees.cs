using System.Threading.Tasks;
using EnterpriseEmployeeManagementInc.Services.Entities;

namespace EnterpriseEmployeeManagementInc.Services
{
    public interface IEmployees
    {
        Task<Employee[]> All(int tenantId);
        Task<Employee> WithId(int tenantId, int employeeId);
        Task<Employee> Add(int tenantId, string firstName, string lastName, string title);
    }
}