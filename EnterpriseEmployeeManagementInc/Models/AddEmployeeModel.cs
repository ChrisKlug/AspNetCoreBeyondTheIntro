using System.ComponentModel.DataAnnotations;

namespace EnterpriseEmployeeManagementInc.Models
{
    public class AddEmployeeModel
    {
        [Required(ErrorMessage = "Have to supply a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Have to supply a last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Have to supply a title")]
        public string Title { get; set; }
    }
}
