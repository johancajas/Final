using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Discrepancy
{
    public int Id { get; set; }

    public int? OrderedItemId { get; set; }

    public int? ReceivedItemId { get; set; }

    public string? Description { get; set; }

    public virtual OrderedItem? OrderedItem { get; set; }

    public virtual ReceivedItem? ReceivedItem { get; set; }
}
