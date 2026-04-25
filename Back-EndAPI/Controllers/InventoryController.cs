using Back_EndAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// GET /api/inventory - Get inventory report for all products
    /// GET /api/inventory?productId=123 - Get inventory report for specific product
    /// </summary>
    /// <param name="productId">Optional product ID to filter inventory</param>
    /// <returns>Inventory report with quantity information</returns>
    /// <response code="200">Returns the inventory report</response>
    /// <response code="404">Product not found (when filtering by productId)</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ClassLibrary.DTOs.Inventory.InventoryReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetInventoryReport([FromQuery] int? productId)
    {
        try
        {
            var report = await _inventoryService.GetInventoryReportAsync(productId);

            // Return 404 if filtering by product ID and no results found
            if (productId.HasValue && !report.Items.Any())
            {
                return NotFound(new { message = $"Product with ID {productId} not found." });
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving inventory report.", error = ex.Message });
        }
    }
}
