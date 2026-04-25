using Back_EndAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back_EndAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CustomerOrderService _customerOrderService;

    public OrdersController(CustomerOrderService customerOrderService)
    {
        _customerOrderService = customerOrderService;
    }

    /// <summary>
    /// GET /api/orders/{id} - Get order status by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order status with items and tracking information</returns>
    /// <response code="200">Returns the order status</response>
    /// <response code="404">Order not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClassLibrary.DTOs.Order.OrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderStatus(int id)
    {
        try
        {
            var orderStatus = await _customerOrderService.GetOrderStatusAsync(id);

            if (orderStatus == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found." });
            }

            return Ok(orderStatus);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving order status.", error = ex.Message });
        }
    }
}
