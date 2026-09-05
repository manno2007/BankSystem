using BankSystem.Web.Data;
using BankSystem.Web.Models;
using BankSystem.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Web.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerByUserIdAsync(string userId)
        {
            return await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<Account>> GetCustomerAccountsAsync(string userId)
        {
            var customer = await GetCustomerByUserIdAsync(userId);
            return customer?.Accounts?.ToList() ?? new List<Account>();
        }

        public async Task CreateCustomerProfileAsync(string userId, string userFirstName, string userLastName, Customer model)
        {
            model.UserId = userId;
            if (string.IsNullOrEmpty(model.FirstName)) model.FirstName = userFirstName;
            if (string.IsNullOrEmpty(model.LastName)) model.LastName = userLastName;

            _context.Customers.Add(model);
            await _context.SaveChangesAsync();
            
            // Auto-create initial checking account
            var account = new Account
            {
                AccountNumber = "CHK" + new Random().Next(1000000, 9999999).ToString(),
                Balance = 0,
                AccountType = "Checking",
                CustomerId = model.Id
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }
    }
}
