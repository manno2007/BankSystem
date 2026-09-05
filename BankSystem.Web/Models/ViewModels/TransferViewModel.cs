using System.ComponentModel.DataAnnotations;

namespace BankSystem.Web.Models.ViewModels
{
    public class TransferViewModel
    {
        [Required]
        public int FromAccountId { get; set; }
        
        [Required]
        [Display(Name = "To Account Number")]
        public string ToAccountNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
        
        public string? Description { get; set; }
    }
}
