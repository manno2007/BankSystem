using BankSystem.Web.Data;
using BankSystem.Web.Models;
using BankSystem.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankSystem.Web.Services
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApplyForLoanAsync(string userId, Loan model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return false;

            model.CustomerId = customer.Id;
            model.Status = "Pending";
            var rate = model.InterestRate / 12 / 100;
            if (rate > 0)
            {
                var emi = (double)model.PrincipalAmount * (double)rate * Math.Pow(1 + (double)rate, model.DurationMonths) / (Math.Pow(1 + (double)rate, model.DurationMonths) - 1);
                model.EMI = (decimal)emi;
            }
            
            _context.Loans.Add(model);
            _context.AuditLogs.Add(new AuditLog { UserId = userId, Action = "Loan Application", Details = $"Applied for loan of {model.PrincipalAmount}" });
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
