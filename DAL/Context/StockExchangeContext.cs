using Common.Entity;
using Microsoft.EntityFrameworkCore;

namespace MagnisStockExchange
{
    public class StockExchangeContext : DbContext
    {
        public DbSet<StockExchangeEntity> StockExchange { get; set; }

        public StockExchangeContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=StockExchangeContext;Trusted_Connection=True;");
        }
    }
}
