using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class AisleBay
{
    public int Id { get; set; }

    public int? BayNumber { get; set; }

    public int? AisleNumber { get; set; }

    public virtual Aisle? AisleNumberNavigation { get; set; }

    public virtual Bay? BayNumberNavigation { get; set; }

    public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();
}
