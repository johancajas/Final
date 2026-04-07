using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs.Shipment;

/// <summary>
/// Request DTO for receiving a shipment
/// </summary>
public class ReceiveShipmentRequest
{
    [Required(ErrorMessage = "Shipment ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Shipment ID must be greater than 0")]
    public int ShipmentId { get; set; }

    [Required(ErrorMessage = "Received date is required")]
    public DateOnly ReceivedDate { get; set; }

    [Required(ErrorMessage = "At least one received item is required")]
    [MinLength(1, ErrorMessage = "At least one received item is required")]
    public List<ReceiveItemRequest> ReceivedItems { get; set; } = new();
}

/// <summary>
/// Request DTO for a received item
/// </summary>
public class ReceiveItemRequest
{
    [Required(ErrorMessage = "Ordered Item ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Ordered Item ID must be greater than 0")]
    public int OrderedItemId { get; set; }

    [Required(ErrorMessage = "Received quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Received quantity must be greater than 0")]
    public int ReceivedQuantity { get; set; }
}
