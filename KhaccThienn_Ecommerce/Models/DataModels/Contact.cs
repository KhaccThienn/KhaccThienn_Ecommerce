using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Contact")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string Name { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(150)")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string Email { get; set; }
        
        [Required]
        [Column(TypeName = "ntext")]
        public string Messsage { get; set; }

        [DefaultValue("getdate()")]
        public DateTime? Created_Date { get; set; }
    }
}
