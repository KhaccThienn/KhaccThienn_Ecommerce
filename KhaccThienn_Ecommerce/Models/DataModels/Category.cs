using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Category Id")]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Column(TypeName = "tinyint")]
        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Column(TypeName = "text")]
        [Display(Name = "Category Image")]
        public string? Image { get; set; }

        [DefaultValue("getdate()")]
        [Display(Name = "Created Date")]
        public DateTime? Created_Date { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? Updated_Date { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
