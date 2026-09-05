using System;
using System.Collections.Generic;

namespace BankSystem.Web.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountType { get; set; } = "Savings";
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
