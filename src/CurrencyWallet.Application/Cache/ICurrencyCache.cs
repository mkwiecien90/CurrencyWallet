namespace CurrencyWallet.Application.Cache
{
    public interface ICurrencyCache
    {
        IList<string> GetAvailableCurrencies();
        void UpdateCache(IEnumerable<string> currencies);
    }
}
