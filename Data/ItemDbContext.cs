using Microsoft.EntityFrameworkCore;
using network_inventory_system.Models;

namespace network_inventory_system.Data
{
    public partial class ItemDbContext : DbContext
    {
        public ItemDbContext()
        {
        }

        public ItemDbContext(DbContextOptions<ItemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
    }
}
