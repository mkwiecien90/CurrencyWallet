#region Usings

using Microsoft.Extensions.Caching.Memory;

#endregion

namespace CurrencyWallet.Application.Cache
{
    public class CurrencyCache : ICurrencyCache
    {
        private readonly IMemoryCache _memoryCache;

        public CurrencyCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public IList<string> GetAvailableCurrencies()
        {
            _memoryCache.TryGetValue("AvailableCurrencies", out IEnumerable<string> currencies);
            return currencies.ToList() ?? new List<string>();
        }

        public void UpdateCache(IEnumerable<string> currencies)
        {
            _memoryCache.Set("AvailableCurrencies", currencies, TimeSpan.FromHours(1));
        }
    }
}
