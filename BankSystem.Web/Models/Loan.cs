using System;

namespace BankSystem.Web.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int DurationMonths { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal EMI { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
