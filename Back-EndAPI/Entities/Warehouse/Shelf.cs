using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Shelf
{
    public char ShelfLetter { get; set; }

    public virtual ICollection<AisleShelf> AisleShelves { get; set; } = new List<AisleShelf>();
}
