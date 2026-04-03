namespace ClassLibrary.DTOs.PurchaseOrder;

public class PurchaseOrderResponse
{
    public int OrderId { get; set; }
    public int SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateOnly DatePurchased { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class OrderItemResponse
{
    public int OrderItemId { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal ActualPrice { get; set; }
    public decimal LineTotal { get; set; }
}
