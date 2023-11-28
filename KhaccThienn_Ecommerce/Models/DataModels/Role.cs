using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int? Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Role Description")]
        public string? Descriptions { get; set; }
        [DefaultValue("getdate()")]
        [Display(Name = "Date Created")]
        public DateTime? Created_Date { get; set; }
        
        [Display(Name = "Date Updated")]
        public DateTime? Updated_Date { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
