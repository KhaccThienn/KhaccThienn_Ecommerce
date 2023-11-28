using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.ViewModels
{
    public class UpdateAccountViewModels 
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        [MinLength(5)]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "New Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "Password Confirmination")]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string? Email { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number Format")]
        public string? Phone { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Avatar { get; set; }


        [Display(Name = "Date Created")]
        public DateTime? Created_Date { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime? Updated_Date { get; set; }

        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}
