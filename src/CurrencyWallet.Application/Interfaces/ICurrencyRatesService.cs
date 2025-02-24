namespace CurrencyWallet.Application.Interfaces
{
    public interface ICurrencyRatesService
    {
        Task UpdateExchangeRatesAsync(CancellationToken cancellationToken);
    }
}
