using System;
using System.Collections.Generic;

namespace BankSystem.Web.Models.ViewModels
{
    public class StatementViewModel
    {
        public int AccountId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
