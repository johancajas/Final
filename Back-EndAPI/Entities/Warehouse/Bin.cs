using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class Bin
{
    public int Id { get; set; }

    public int? SkuNumber { get; set; }

    public int? Qtystored { get; set; }

    public int? AisleBayId { get; set; }

    public int? AisleShelfId { get; set; }

    public int? Height { get; set; }

    public int? Volume { get; set; }

    public virtual AisleBay? AisleBay { get; set; }

    public virtual AisleShelf? AisleShelf { get; set; }

    public virtual Item? SkuNumberNavigation { get; set; }

    public virtual ICollection<TransferRecord> TransferRecords { get; set; } = new List<TransferRecord>();
}
