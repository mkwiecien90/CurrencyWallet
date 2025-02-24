#region Usings

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

#endregion

namespace CurrencyWallet.Infrastructure.Persistance
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        protected IConfiguration Configuration { get; }

        public DesignTimeDbContextFactory() { }

        public DesignTimeDbContextFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}