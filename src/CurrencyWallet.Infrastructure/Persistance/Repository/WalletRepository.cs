#region Usings

using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace CurrencyWallet.Infrastructure.Persistance.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            _context.SaveChanges();
            return wallet.Id;
        }

        public async Task<Wallet> GetByIdAsync(Guid id)
        {
            return await _context.Wallets.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Wallet>> GetAll()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Wallet wallet)
        {
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }
    }

}
