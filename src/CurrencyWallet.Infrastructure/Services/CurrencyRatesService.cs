#region Usings

using CurrencyWallet.Application.Cache;
using CurrencyWallet.Application.DTOs;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Common;
using CurrencyWallet.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#endregion

namespace CurrencyWallet.Infrastructure.Services
{
    public class CurrencyRatesService : BackgroundService, ICurrencyRatesService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CurrencyRatesService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrencyCache _currencyCache;

        public CurrencyRatesService(IServiceScopeFactory serviceScopeFactory, ILogger<CurrencyRatesService> logger, IHttpClientFactory httpClientFactory, ICurrencyCache currencyCache)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _currencyCache = currencyCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExchangeRateBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var currencyRatesService = scope.ServiceProvider.GetRequiredService<ICurrencyRatesService>();
                    await currencyRatesService.UpdateExchangeRatesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating exchange rates.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public async Task UpdateExchangeRatesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var currencyRateRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRateRepository>();

            var rates = await FetchExchangeRatesAsync();
            if (rates != null)
            {
                await currencyRateRepository.UpdateRatesAsync(rates);
                var currencyCodes = rates?.Select(s => s.Code);
                if (currencyCodes is not null)
                {
                    _currencyCache.UpdateCache(currencyCodes);
                }
                _logger.LogInformation("Currency rates updated.");
            }

            _logger.LogInformation("Exchange rates updated successfully.");
        }

        private async Task<List<CurrencyRate>?> FetchExchangeRatesAsync()
        {
            var client = _httpClientFactory.CreateClient(ConstConfig.NbpApi);
            var response = await client.GetStringAsync("exchangerates/tables/B?format=json");
            var tables = JsonConvert.DeserializeObject<List<ExchangeRatesTable>>(response);
            return tables?[0].Rates;
        }
    }
}
