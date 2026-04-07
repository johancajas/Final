using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class ReceivedShipment
{
    public int Id { get; set; }

    public int? PurchaseOrderId { get; set; }

    public DateOnly? Date { get; set; }

    public string Status { get; set; } = "PENDING";

    public virtual PurchaseOrder? PurchaseOrder { get; set; }

    public virtual ICollection<ReceivedItem> ReceivedItems { get; set; } = new List<ReceivedItem>();
}
