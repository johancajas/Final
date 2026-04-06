using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_EndAPI.Entities;

[Table("shipment_receipt")]
public partial class ShipmentReceipt
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("receiveddate")]
    public DateOnly? Receiveddate { get; set; }

    [Column("receivedby")]
    [StringLength(100)]
    public string? Receivedby { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [ForeignKey("Orderid")]
    [InverseProperty("ShipmentReceipts")]
    public virtual StoreOrder? Order { get; set; }

    [InverseProperty("Receipt")]
    public virtual ICollection<ShipmentReceiptItem> ShipmentReceiptItems { get; set; } = new List<ShipmentReceiptItem>();
}
