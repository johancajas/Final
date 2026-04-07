using Back_EndAPI.Services;
using ClassLibrary.DTOs.PurchaseOrder;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderController : ControllerBase
{
    private readonly PurchaseOrderService _service;

    public PurchaseOrderController(PurchaseOrderService service)
    {
        _service = service;
    }

    // ============================================================
    // CREATE PURCHASE ORDER
    // POST: api/purchaseorder
    // ============================================================
    /// <summary>
    /// Creates a new purchase order with status CREATED
    /// </summary>
    /// <remarks>
    /// Business Rules:
    /// - Must include at least one item
    /// - Each item must have productId and quantity > 0
    /// - Initial status is automatically set to CREATED
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderResponse), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        // Model validation handled automatically by [ApiController] attribute
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
            var result = await _service.CreatePurchaseOrderAsync(request);
            return CreatedAtAction(
                nameof(GetPurchaseOrderById), 
                new { id = result.OrderId }, 
                result
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                error = "An error occurred while creating the purchase order", 
                details = ex.Message 
            });
        }
    }

    // ============================================================
    // GET PURCHASE ORDER BY ID
    // GET: api/purchaseorder/{id}
    // ============================================================
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderResponse), 200)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetPurchaseOrderById(int id)
    {
        try
        {
            var result = await _service.GetPurchaseOrderByIdAsync(id);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }

    // ============================================================
    // GET ALL PURCHASE ORDERS
    // GET: api/purchaseorder
    // ============================================================
    [HttpGet]
    [ProducesResponseType(typeof(List<PurchaseOrderResponse>), 200)]
    public async Task<IActionResult> GetAllPurchaseOrders()
    {
        try
        {
            var result = await _service.GetAllPurchaseOrdersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }

    // ============================================================
    // UPDATE PURCHASE ORDER STATUS
    // PUT: api/purchaseorder/{id}/status
    // ============================================================
    [HttpPut("{id}/status")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            await _service.UpdatePurchaseOrderStatusAsync(id, request.Status);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred", details = ex.Message });
        }
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
