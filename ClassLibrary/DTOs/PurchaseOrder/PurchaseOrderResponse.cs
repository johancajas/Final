namespace ClassLibrary.DTOs.PurchaseOrder;

/// <summary>
/// Response DTO for a created or retrieved purchase order
/// </summary>
public class PurchaseOrderResponse
{
    public int OrderId { get; set; }
    public int VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateOnly DateOrdered { get; set; }
    public string Status { get; set; } = "CREATED";
    public List<OrderedItemResponse> Items { get; set; } = new();
    public decimal ExpectedTotalCost { get; set; }
}

/// <summary>
/// Response DTO for an ordered item
/// </summary>
public class OrderedItemResponse
{
    public int OrderedItemId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal CostPerUnit { get; set; }
    public decimal LineTotal { get; set; }
}
