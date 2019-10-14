using System.Threading.Tasks;
using EnterpriseEmployeeManagementInc.Services.Entities;

namespace EnterpriseEmployeeManagementInc.Services
{
    public interface IUsers
    {
        Task<User> Authenticate(string username, string password);
    }
}