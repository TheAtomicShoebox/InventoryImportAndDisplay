using Inventory.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Common.Context;

public partial class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.ItemNo).ValueGeneratedNever();
        });
    }
}
