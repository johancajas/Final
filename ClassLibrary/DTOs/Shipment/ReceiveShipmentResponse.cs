namespace ClassLibrary.DTOs.Shipment;

/// Response DTO for a received shipment
public class ReceiveShipmentResponse
{
    public int ShipmentId { get; set; }
    public int PurchaseOrderId { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public string Status { get; set; } = "RECEIVED";
    public List<ReceivedItemDetail> ReceivedItems { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

/// Detail of a received item
public class ReceivedItemDetail
{
    public int ReceivedItemId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int QuantityDifference { get; set; }
    public string ReceivingStatus { get; set; } = string.Empty; 
}

/// Inventory status after receiving
public class InventoryStatus
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int QuantityAdded { get; set; }
    public int NewTotalQuantity { get; set; }
}
