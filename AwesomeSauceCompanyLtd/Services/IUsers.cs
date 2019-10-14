using System.Threading.Tasks;
using AwesomeSauceCompanyLtd.Services.Entities;

namespace AwesomeSauceCompanyLtd.Services
{
    public interface IUsers
    {
        Task<User> WhereIdIs(int id);
    }
}