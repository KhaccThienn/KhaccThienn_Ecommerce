using KhaccThienn_Ecommerce.Models.DataModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.ViewModels
{
    public class CreateAccountViewModels
    {
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Display(Name = "Username")]
        [MinLength(5)]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Password Confirmination")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? Created_Date { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime? Updated_Date { get; set; }

        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}
