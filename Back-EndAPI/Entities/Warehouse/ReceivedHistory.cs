using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class ReceivedHistory
{
    public int Id { get; set; }

    public int? ReceivedItemId { get; set; }

    public int Qty { get; set; }

    public string? Comment { get; set; }

    public virtual ReceivedItem? ReceivedItem { get; set; }
}
