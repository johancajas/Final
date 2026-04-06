using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_EndAPI.Entities;

[Table("inventory_transaction")]
public partial class InventoryTransaction
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("itemid")]
    public int? Itemid { get; set; }

    [Column("transactiontype")]
    [StringLength(20)]
    public string? Transactiontype { get; set; } // "RECEIVE", "SELL", "ADJUST"

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("transactiondate")]
    public DateOnly? Transactiondate { get; set; }

    [Column("referenceid")]
    public int? Referenceid { get; set; } // References order, receipt, or checkout

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("Itemid")]
    [InverseProperty("InventoryTransactions")]
    public virtual StoreItem? Item { get; set; }
}
