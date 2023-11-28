using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaccThienn_Ecommerce.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Display(Name = "ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        [Required]
        [MinLength(10)]
        [Display(Name = "Deliver Address")]
        public string DeliverAddress { get; set; }

        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number Format")]
        [Display(Name = "Deliver Phone")]
        public string DeliverPhone { get; set; }

        [Required]
        [Display(Name = "Deliver User")]
        public string DeliverUser { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? UserId { get; set; }
        public DateTime? Created_Date { get; set; }
    }
}
