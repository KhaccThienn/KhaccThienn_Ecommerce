using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Username")]
        [MinLength(5)]
        public string Username { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Column(TypeName = "varchar(150)")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string? Email { get; set; }

        [Column(TypeName = "varchar(50)")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number Format")]
        public string? Phone { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Avatar { get; set; }

        [Display(Name = "Role")]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }


        [DefaultValue("getdate()")]
        [Display(Name = "Created Date")]
        public DateTime? Created_Date { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? Updated_Date { get; set; }

        public ICollection<Order>? Orders { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<Cart>? Carts { get; set; }
    }
}
