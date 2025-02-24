#region Usings

using CurrencyWallet.Domain.Entities;

#endregion

namespace CurrencyWallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Guid> AddAsync(Wallet wallet);
        Task<Wallet> GetByIdAsync(Guid id);
        Task<IEnumerable<Wallet>> GetAll();
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(Wallet wallet);
    }
}
