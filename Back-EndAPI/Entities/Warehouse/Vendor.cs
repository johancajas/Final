using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Vendor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
