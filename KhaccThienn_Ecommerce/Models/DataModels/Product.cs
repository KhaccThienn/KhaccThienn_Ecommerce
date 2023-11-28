using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(200)")]
        [Display(Name = "Image")]
        public string? Image { get; set; }

        [Column]
        [Display(Name = "Product Price")]
        public decimal Price { get; set; }

        [Column]
        [Display(Name = "Product Sale Price")]
        [DefaultValue(0)]
        public decimal? SalePrice { get; set; }

        [Column(TypeName = "tinyint")]
        [Display(Name = "Product Status")]
        [DefaultValue(1)]
        public byte Status { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Descriptions")]
        public string? Descriptions { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [DefaultValue("getdate()")]
        [Display(Name = "Date Created")]
        public DateTime? Created_Date { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? Updated_Date { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
