using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_EndAPI.Entities;

[Table("shipment_receipt_item")]
public partial class ShipmentReceiptItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("receiptid")]
    public int? Receiptid { get; set; }

    [Column("orderitemid")]
    public int? Orderitemid { get; set; }

    [Column("receivedquantity")]
    public int? Receivedquantity { get; set; }

    [ForeignKey("Receiptid")]
    [InverseProperty("ShipmentReceiptItems")]
    public virtual ShipmentReceipt? Receipt { get; set; }

    [ForeignKey("Orderitemid")]
    [InverseProperty("ShipmentReceiptItems")]
    public virtual StoreOrderItem? Orderitem { get; set; }
}
