using System;
using System.Collections.Generic;
using Inventory.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.ApiService.Context;

public partial class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Item { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.ItemNo).ValueGeneratedNever();
        });
    }
}
