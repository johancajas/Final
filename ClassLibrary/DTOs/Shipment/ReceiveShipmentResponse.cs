namespace ClassLibrary.DTOs.Shipment;

public class ReceiveShipmentResponse
{
    public int PurchaseOrderId { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public string? SupplierName { get; set; }
    public List<ReceivedItemDetail> ReceivedItems { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ReceivedItemDetail
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int QuantityDifference { get; set; }
    public decimal ActualPrice { get; set; }
    public int NewInventoryCount { get; set; }
    public string Status { get; set; } = string.Empty; // "Complete", "Partial", "Over"
}
