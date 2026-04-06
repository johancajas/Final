using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Item
{
    public int SkuNumber { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? SuggestedSellingPrice { get; set; }

    public int? VolumePerUnit { get; set; }

    public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual ICollection<ReceivedItem> ReceivedItems { get; set; } = new List<ReceivedItem>();
}
