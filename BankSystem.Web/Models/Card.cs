using System;

namespace BankSystem.Web.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; } = string.Empty;
        public string CardType { get; set; } = "Debit";
        public bool IsActive { get; set; } = true;
    }
}
