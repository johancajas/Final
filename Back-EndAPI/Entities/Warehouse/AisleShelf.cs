using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class AisleShelf
{
    public int Id { get; set; }

    public char? ShelfLetter { get; set; }

    public int? AisleNumber { get; set; }

    public int? ShelfHeight { get; set; }

    public virtual Aisle? AisleNumberNavigation { get; set; }

    public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();

    public virtual Shelf? ShelfLetterNavigation { get; set; }
}
