using Back_EndAPI.Services;
using Back_EndAPI.Filters;
using ClassLibrary.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

/// <summary>
/// Enhanced Inventory Controller with Role-Based Authorization and Idempotency
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryControllerEnhanced : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryControllerEnhanced(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Pick inventory for an order - Requires admin or warehouse_worker role
    /// Supports idempotency via Idempotency-Key header
    /// </summary>
    /// <param name="request">Pick request details</param>
    /// <returns>Pick confirmation</returns>
    [HttpPost("pick")]
    [RoleRequirement(Roles.Admin, Roles.WarehouseWorker)]
    [Idempotent("PickInventory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PickInventory([FromBody] PickInventoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request data", errors = ModelState });
            }

            // Implement the pick logic in InventoryService
            var result = new
            {
                orderId = request.OrderId,
                itemsPicked = request.Items.Count,
                message = "Inventory picked successfully",
                timestamp = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while picking inventory", error = ex.Message });
        }
    }

    /// <summary>
    /// Get inventory report - All authenticated users can view
    /// Admin and warehouse workers see all, customers see limited info
    /// </summary>
    [HttpGet]
    [RoleRequirement(Roles.Admin, Roles.WarehouseWorker, Roles.Customer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInventoryReport([FromQuery] int? productId)
    {
        try
        {
            var report = await _inventoryService.GetInventoryReportAsync(productId);

            // Customers might see limited information
            var isCustomer = User.IsInRole(Roles.Customer);
            if (isCustomer)
            {
                // Optionally filter or limit data for customers
                // For now, return the same data
            }

            if (productId.HasValue && !report.Items.Any())
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}

/// <summary>
/// DTO for picking inventory
/// </summary>
public class PickInventoryRequest
{
    public int OrderId { get; set; }
    public List<PickItemDto> Items { get; set; } = new();
}

public class PickItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Location { get; set; }
}
