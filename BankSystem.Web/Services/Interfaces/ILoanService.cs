using BankSystem.Web.Models;
using System.Threading.Tasks;

namespace BankSystem.Web.Services.Interfaces
{
    public interface ILoanService
    {
        Task<bool> ApplyForLoanAsync(string userId, Loan model);
    }
}
