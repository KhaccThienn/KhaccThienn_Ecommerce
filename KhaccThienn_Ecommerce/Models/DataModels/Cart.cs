using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Cart Id")]
        public int ID { get; set; }

        [Column]
        [Display(Name = "Quantity")]
        public int? Quantity { get; set; }

        [Column]
        [Display(Name = "Total Price")]
        public decimal? Total { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public int? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [DefaultValue("getdate()")]
        [Display(Name = "Date Created")]
        public DateTime? Created_Date { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime? Updated_Date { get; set; }
    }
}
