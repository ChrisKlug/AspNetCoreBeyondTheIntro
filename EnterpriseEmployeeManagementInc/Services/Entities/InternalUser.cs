namespace EnterpriseEmployeeManagementInc.Services.Entities
{
    public class InternalUser : User
    {
        public User AsUser()
        {
            return new User
            {
                Id = Id,
                Tenant = Tenant,
                Username = Username,
                Name = Name
            };
        }

        public string Password { get; set; }
    }
}
