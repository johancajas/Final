using Back_EndAPI.Services;
using ClassLibrary.DTOs.Shipment;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentController : ControllerBase
{
    private readonly ShipmentService _service;

    public ShipmentController(ShipmentService service)
    {
        _service = service;
    }

    // ============================================================
    // RECEIVE SHIPMENT
    // POST: api/shipment/receive
    // ============================================================
    
    [HttpPost("receive")]
    [ProducesResponseType(typeof(ReceiveShipmentResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 409)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> ReceiveShipment([FromBody] ReceiveShipmentRequest request)
    {
        // Model validation handled automatically by [ApiController]
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                error = "Validation failed",
                details = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        try
        {
            var result = await _service.ReceiveShipmentAsync(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            // Shipment does not exist → 404 Not Found
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Check if it's a duplicate receiving attempt
            if (ex.Message.Contains("already been received"))
            {
                // Shipment already received → 409 Conflict
                return Conflict(new { error = ex.Message });
            }

            // Other business rule violations → 400 Bad Request
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "An error occurred while receiving the shipment",
                details = ex.Message
            });
        }
    }

    // ============================================================
    // GET SHIPMENT BY ID
    // GET: api/shipment/{id}
    // ============================================================
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReceiveShipmentResponse), 200)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetShipmentById(int id)
    {
        try
        {
            var result = await _service.GetShipmentByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { error = $"Shipment {id} not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }

    // ============================================================
    // GET ALL SHIPMENTS
    // GET: api/shipment
    // ============================================================
    [HttpGet]
    [ProducesResponseType(typeof(List<ReceiveShipmentResponse>), 200)]
    public async Task<IActionResult> GetAllShipments()
    {
        try
        {
            var result = await _service.GetAllShipmentsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }

    // ============================================================
    // GET INVENTORY REPORT
    // GET: api/shipment/inventory
    // ============================================================
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(List<InventoryReportItem>), 200)]
    public async Task<IActionResult> GetInventoryReport()
    {
        try
        {
            var result = await _service.GetInventoryReportAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }
}
