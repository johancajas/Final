using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class ReceivedItem
{
    public int Id { get; set; }

    public int? SkuNumber { get; set; }

    public int? ShipmentId { get; set; }

    public int? Qty { get; set; }

    public virtual ICollection<Discrepancy> Discrepancies { get; set; } = new List<Discrepancy>();

    public virtual ICollection<ReceivedHistory> ReceivedHistories { get; set; } = new List<ReceivedHistory>();

    public virtual ReceivedShipment? Shipment { get; set; }

    public virtual Item? SkuNumberNavigation { get; set; }

    public virtual ICollection<TransferRecord> TransferRecords { get; set; } = new List<TransferRecord>();
}
