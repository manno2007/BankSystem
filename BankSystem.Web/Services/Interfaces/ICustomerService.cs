using BankSystem.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSystem.Web.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByUserIdAsync(string userId);
        Task<List<Account>> GetCustomerAccountsAsync(string userId);
        Task CreateCustomerProfileAsync(string userId, string userFirstName, string userLastName, Customer model);
    }
}
