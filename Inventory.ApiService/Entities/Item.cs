using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.ApiService.Entities;

public partial class Item
{
    [Key]
    public long ItemNo { get; set; }

    public string ItemDescription { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal Price { get; set; }
}
