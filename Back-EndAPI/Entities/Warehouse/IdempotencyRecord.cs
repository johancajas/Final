namespace Back_EndAPI.Entities.Warehouse;

/// <summary>
/// Entity for tracking idempotent requests to prevent duplicate processing
/// </summary>
public class IdempotencyRecord
{
    public int Id { get; set; }

    /// <summary>
    /// Unique idempotency key from request header
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// Name of the operation (e.g., ReceiveShipment, PickInventory, ShipOrder)
    /// </summary>
    public string OperationName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the request was first processed
    /// </summary>
    public DateTime ProcessedAt { get; set; }

    /// <summary>
    /// HTTP status code of the original response
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Serialized response body for consistency
    /// </summary>
    public string ResponseBody { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Reference to the entity ID created/modified
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// User who initiated the request
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Expiration time for the record (for cleanup)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
