using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Aisle
{
    public int AisleNumber { get; set; }

    public virtual ICollection<AisleBay> AisleBays { get; set; } = new List<AisleBay>();

    public virtual ICollection<AisleShelf> AisleShelves { get; set; } = new List<AisleShelf>();
}
