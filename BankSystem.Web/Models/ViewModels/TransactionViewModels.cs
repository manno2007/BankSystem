using System.ComponentModel.DataAnnotations;

namespace BankSystem.Web.Models.ViewModels
{
    public class DepositViewModel
    {
        [Required]
        [Display(Name = "Account")]
        public int AccountId { get; set; }

        [Required]
        [Range(1, 1000000, ErrorMessage = "Amount must be between 1 and 1,000,000")]
        public decimal Amount { get; set; }
    }

    public class WithdrawViewModel
    {
        [Required]
        [Display(Name = "Account")]
        public int AccountId { get; set; }

        [Required]
        [Range(1, 1000000, ErrorMessage = "Amount must be between 1 and 1,000,000")]
        public decimal Amount { get; set; }
    }
}
