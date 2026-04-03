using Back_EndAPI.Services;
using ClassLibrary.DTOs.PurchaseOrder;
using Microsoft.AspNetCore.Authorization;
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
    [HttpPost]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        // Model validation happens automatically due to [ApiController] attribute
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
            return StatusCode(500, new { error = "An error occurred while creating the purchase order", details = ex.Message });
        }
    }

    // ============================================================
    // GET PURCHASE ORDER BY ID
    // GET: api/purchaseorder/{id}
    // ============================================================
    [HttpGet("{id}")]
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
            return StatusCode(500, new { error = "An error occurred while retrieving the purchase order", details = ex.Message });
        }
    }

    // ============================================================
    // GET ALL PURCHASE ORDERS
    // GET: api/purchaseorder
    // ============================================================
    [HttpGet]
    public async Task<IActionResult> GetAllPurchaseOrders()
    {
        try
        {
            var result = await _service.GetAllPurchaseOrdersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving purchase orders", details = ex.Message });
        }
    }
}
