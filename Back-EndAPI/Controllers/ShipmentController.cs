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
    // RECEIVE SHIPMENT AND UPDATE INVENTORY
    // POST: api/shipment/receive
    // ============================================================
    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveShipment([FromBody] ReceiveShipmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _service.ReceiveShipmentAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while receiving the shipment", details = ex.Message });
        }
    }

    // ============================================================
    // GET INVENTORY REPORT
    // GET: api/shipment/inventory
    // ============================================================
    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventoryReport()
    {
        try
        {
            var result = await _service.GetInventoryReportAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving inventory report", details = ex.Message });
        }
    }
}
