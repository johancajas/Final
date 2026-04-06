using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs.Shipment;

public class ReceiveShipmentRequest
{
    [Required(ErrorMessage = "Purchase Order ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Purchase Order ID must be greater than 0")]
    public int PurchaseOrderId { get; set; }

    [Required(ErrorMessage = "Received date is required")]
    public DateOnly ReceivedDate { get; set; }

    [Required(ErrorMessage = "At least one received item is required")]
    [MinLength(1, ErrorMessage = "At least one received item is required")]
    public List<ReceivedItemRequest> ReceivedItems { get; set; } = new();
}

public class ReceivedItemRequest
{
    [Required(ErrorMessage = "Order Item ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Order Item ID must be greater than 0")]
    public int OrderItemId { get; set; }

    [Required(ErrorMessage = "Received quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Received quantity must be at least 1")]
    public int ReceivedQuantity { get; set; }
}
