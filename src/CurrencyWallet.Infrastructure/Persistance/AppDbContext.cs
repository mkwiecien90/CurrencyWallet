#region Usings

using CurrencyWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace CurrencyWallet.Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
            .OwnsMany(w => w.Balances, balances =>
            {
                balances.OwnsOne(b => b.Money, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("Amount");
                    money.Property(m => m.Currency).HasColumnName("Currency");
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
