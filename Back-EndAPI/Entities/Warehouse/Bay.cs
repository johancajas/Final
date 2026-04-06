using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Bay
{
    public int BayNumber { get; set; }

    public virtual ICollection<AisleBay> AisleBays { get; set; } = new List<AisleBay>();
}
