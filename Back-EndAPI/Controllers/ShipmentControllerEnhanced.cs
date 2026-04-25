using Back_EndAPI.Services;
using Back_EndAPI.Filters;
using ClassLibrary.Authorization;
using ClassLibrary.DTOs.Shipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

/// <summary>
/// Enhanced Shipment Controller with Role-Based Authorization and Idempotency
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class ShipmentControllerEnhanced : ControllerBase
{
    private readonly ShipmentService _shipmentService;

    public ShipmentControllerEnhanced(ShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    /// <summary>
    /// Receive a shipment - Requires admin or warehouse_worker role
    /// Supports idempotency via Idempotency-Key header
    /// </summary>
    /// <param name="request">Shipment receipt details</param>
    /// <returns>Receipt confirmation with received items</returns>
    /// <response code="200">Shipment received successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Purchase order not found</response>
    [HttpPost("receive")]
    [RoleRequirement(Roles.Admin, Roles.WarehouseWorker)]
    [Idempotent("ReceiveShipment")]
    [ProducesResponseType(typeof(ReceiveShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceiveShipment([FromBody] ReceiveShipmentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data", errors = ModelState });
            }

            var result = await _shipmentService.ReceiveShipmentAsync(request);

            if (result == null)
            {
                return NotFound(new { message = "Purchase order not found or already received" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while receiving shipment", error = ex.Message });
        }
    }

    /// <summary>
    /// Get shipment details - Accessible by admin, warehouse_worker, and customer
    /// </summary>
    /// <param name="id">Shipment ID</param>
    /// <returns>Shipment details</returns>
    [HttpGet("{id}")]
    [RoleRequirement(Roles.Admin, Roles.WarehouseWorker, Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShipment(int id)
    {
        try
        {
            // Note: If implementing customer filtering, add logic here to check
            // that the customer can only view their own shipments
            var userName = User.Identity?.Name;
            var isCustomer = User.IsInRole(Roles.Customer);

            // Example: Customer can only view their own orders
            // Implement this based on your business logic

            return Ok(new { message = "Shipment details endpoint", id, user = userName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}
