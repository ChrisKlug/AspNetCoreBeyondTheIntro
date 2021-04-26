using AwesomeSauceCompanyLtd.Services.Entities;

namespace AwesomeSauceCompanyLtd.Controllers.Api.Models
{
    public class BasicUser
    {
        public static BasicUser Create(User user)
        {
            return new BasicUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}