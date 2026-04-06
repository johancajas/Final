using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class PurchaseOrder
{
    public int Id { get; set; }

    public DateOnly DateOrdered { get; set; }

    public int? Vendorid { get; set; }

    public int? ExpectedTotalCost { get; set; }

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual ICollection<ReceivedShipment> ReceivedShipments { get; set; } = new List<ReceivedShipment>();

    public virtual Vendor? Vendor { get; set; }
}
