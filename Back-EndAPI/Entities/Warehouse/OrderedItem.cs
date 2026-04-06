using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class OrderedItem
{
    public int Id { get; set; }

    public int? SkuNumber { get; set; }

    public int? PurchaseId { get; set; }

    public int Qty { get; set; }

    public decimal CostPerUnit { get; set; }

    public virtual ICollection<Discrepancy> Discrepancies { get; set; } = new List<Discrepancy>();

    public virtual PurchaseOrder? Purchase { get; set; }

    public virtual Item? SkuNumberNavigation { get; set; }
}
