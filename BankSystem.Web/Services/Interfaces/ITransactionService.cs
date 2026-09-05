using BankSystem.Web.Models.ViewModels;
using System.Threading.Tasks;

namespace BankSystem.Web.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<(bool IsSuccess, string ErrorMessage)> TransferAsync(string userId, TransferViewModel model);
        Task<(bool IsSuccess, string ErrorMessage)> DepositAsync(string userId, DepositViewModel model);
        Task<(bool IsSuccess, string ErrorMessage)> WithdrawAsync(string userId, WithdrawViewModel model);
    }
}
