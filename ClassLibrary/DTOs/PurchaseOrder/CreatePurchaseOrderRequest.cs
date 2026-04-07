using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs.PurchaseOrder;

/// <summary>
/// Request DTO for creating a new purchase order
/// </summary>
public class CreatePurchaseOrderRequest
{
    [Required(ErrorMessage = "Vendor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Vendor ID must be greater than 0")]
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Order date is required")]
    public DateOnly DateOrdered { get; set; }

    [Required(ErrorMessage = "At least one item is required")]
    [MinLength(1, ErrorMessage = "Purchase order must contain at least one item")]
    public List<CreateOrderedItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for an item in a purchase order
/// </summary>
public class CreateOrderedItemRequest
{
    [Required(ErrorMessage = "Product ID (SKU Number) is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Cost per unit is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Cost per unit must be greater than 0")]
    public decimal CostPerUnit { get; set; }
}
