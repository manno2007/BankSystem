using BankSystem.Web.Data;
using BankSystem.Web.Models;
using BankSystem.Web.Models.ViewModels;
using BankSystem.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankSystem.Web.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> TransferAsync(string userId, TransferViewModel model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return (false, "Customer not found.");

            var fromAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.FromAccountId && a.CustomerId == customer.Id);
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == model.ToAccountNumber);

            if (fromAccount == null || toAccount == null) return (false, "Invalid account details.");
            if (fromAccount.Balance < model.Amount) return (false, "Insufficient funds.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                fromAccount.Balance -= model.Amount;
                toAccount.Balance += model.Amount;

                var txOut = new Transaction { AccountId = fromAccount.Id, Amount = model.Amount, Type = "Transfer-Out", Description = $"To {toAccount.AccountNumber}" };
                var txIn = new Transaction { AccountId = toAccount.Id, Amount = model.Amount, Type = "Transfer-In", Description = $"From {fromAccount.AccountNumber}" };

                _context.Transactions.AddRange(txOut, txIn);
                
                var log = new AuditLog { UserId = userId, Action = "Transfer", Details = $"Transferred {model.Amount} from {fromAccount.AccountNumber} to {toAccount.AccountNumber}" };
                _context.AuditLogs.Add(log);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (false, "Transfer failed.");
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DepositAsync(string userId, DepositViewModel model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return (false, "Customer not found.");

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.CustomerId == customer.Id);
            if (account == null) return (false, "Invalid account.");

            account.Balance += model.Amount;
            var tx = new Transaction { AccountId = account.Id, Amount = model.Amount, Type = "Deposit", Description = "Cash Deposit" };
            _context.Transactions.Add(tx);
            
            var log = new AuditLog { UserId = userId, Action = "Deposit", Details = $"Deposited {model.Amount} to {account.AccountNumber}" };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> WithdrawAsync(string userId, WithdrawViewModel model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return (false, "Customer not found.");

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.CustomerId == customer.Id);
            if (account == null) return (false, "Invalid account.");

            if (account.Balance < model.Amount) return (false, "Insufficient funds.");

            account.Balance -= model.Amount;
            var tx = new Transaction { AccountId = account.Id, Amount = model.Amount, Type = "Withdraw", Description = "Cash Withdrawal" };
            _context.Transactions.Add(tx);
            
            var log = new AuditLog { UserId = userId, Action = "Withdraw", Details = $"Withdrew {model.Amount} from {account.AccountNumber}" };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
