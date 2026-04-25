namespace ClassLibrary.DTOs.Order;

/// <summary>
/// Response DTO for order status report containing complete order information
/// </summary>
public class OrderStatusResponse
{
    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Human-readable order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the order (Pending, Picking, Partially Received, Completed)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Date when the order was created
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Name of the customer or vendor (optional)
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// List of items in the order
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Tracking number for shipment (if shipped)
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// Shipping carrier name (if shipped)
    /// </summary>
    public string? ShippingCarrier { get; set; }

    /// <summary>
    /// Date when the order was shipped (if shipped)
    /// </summary>
    public DateTime? ShippedDate { get; set; }

    /// <summary>
    /// Estimated delivery date (if shipped)
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; set; }
}

/// <summary>
/// DTO representing a single item in an order
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int QuantityOrdered { get; set; }

    /// <summary>
    /// Quantity that has been picked
    /// </summary>
    public int QuantityPicked { get; set; }

    /// <summary>
    /// Unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }
}
