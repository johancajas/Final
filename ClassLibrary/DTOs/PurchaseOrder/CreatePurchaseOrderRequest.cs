using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs.PurchaseOrder;

public class CreatePurchaseOrderRequest
{
    [Required(ErrorMessage = "Supplier ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be greater than 0")]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Date purchased is required")]
    public DateOnly DatePurchased { get; set; }

    [Required(ErrorMessage = "At least one order item is required")]
    [MinLength(1, ErrorMessage = "At least one order item is required")]
    public List<CreateOrderItemRequest> OrderItems { get; set; } = new();
}

public class CreateOrderItemRequest
{
    [Required(ErrorMessage = "Item ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Item ID must be greater than 0")]
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Actual price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Actual price must be greater than 0")]
    public decimal ActualPrice { get; set; }
}
