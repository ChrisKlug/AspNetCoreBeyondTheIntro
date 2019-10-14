namespace EnterpriseEmployeeManagementInc.Services.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Picture { get; set; }
        public string Thumbnail { get; set; }
    }
}
