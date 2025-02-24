#region Usings

using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace CurrencyWallet.Infrastructure.Persistance.Repository
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateRatesAsync(IEnumerable<CurrencyRate> rates)
        {
            _context.CurrencyRates.RemoveRange(_context.CurrencyRates);
            _context.CurrencyRates.AddRange(rates);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CurrencyRate>> GetAllAsync()
        {
            return await _context.CurrencyRates.ToListAsync();
        }
    }
}
