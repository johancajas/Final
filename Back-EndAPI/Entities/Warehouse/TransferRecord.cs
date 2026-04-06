using System;
using System.Collections.Generic;

namespace Back_EndAPI.Entities.Warehouse;

public partial class TransferRecord
{
    public int Id { get; set; }

    public int? Storagelocationid { get; set; }

    public bool? Withdrawl { get; set; }

    public bool? Deposit { get; set; }

    public int? Qty { get; set; }

    public int? Receiveditemid { get; set; }

    public DateTime? Datetime { get; set; }

    public virtual ReceivedItem? Receiveditem { get; set; }

    public virtual Bin? Storagelocation { get; set; }
}
