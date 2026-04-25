namespace ClassLibrary.DTOs.Inventory;

/// <summary>
/// Response DTO for inventory report containing list of inventory items
/// </summary>
public class InventoryReportResponse
{
    /// <summary>
    /// List of inventory items with availability information
    /// </summary>
    public List<InventoryItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO representing a single inventory item with quantity information
/// </summary>
public class InventoryItemDto
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
    /// Description of the product (optional)
    /// </summary>
    public string? ProductDescription { get; set; }

    /// <summary>
    /// Total quantity available (QuantityReceived - QuantityPicked)
    /// </summary>
    public int TotalQuantityAvailable { get; set; }

    /// <summary>
    /// Total quantity received from all shipments
    /// </summary>
    public int QuantityReceived { get; set; }

    /// <summary>
    /// Total quantity picked for orders
    /// </summary>
    public int QuantityPicked { get; set; }
}
