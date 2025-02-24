#region Usings

using CurrencyWallet.Domain.Entities;

#endregion

namespace CurrencyWallet.Application.Interfaces
{
    public interface ICurrencyRateRepository
    {
        Task UpdateRatesAsync(IEnumerable<CurrencyRate> rates);
        Task<IEnumerable<CurrencyRate>> GetAllAsync();
    }
}
