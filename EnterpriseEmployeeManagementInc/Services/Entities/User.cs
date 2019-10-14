namespace EnterpriseEmployeeManagementInc.Services.Entities
{
    public class User
    {
        public int Id { get; set; }
        public Tenant Tenant { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
    }
}
