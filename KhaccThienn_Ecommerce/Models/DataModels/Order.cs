using KhaccThienn_Ecommerce.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.DataModels
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string DeliverAddress { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string DeliverPhone { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string DeliverUser { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string? Note { get; set; }

        [Column(TypeName = "tinyint")]
        public OrderStatuses Status { get; set; }
        
        [Column]
        public decimal TotalPrice { get; set; }
        
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [DefaultValue("getdate()")]
        public DateTime? Created_Date { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
