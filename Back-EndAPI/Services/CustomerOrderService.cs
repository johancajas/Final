using Back_EndAPI.Data;
using ClassLibrary.DTOs.Order;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class CustomerOrderService
{
    private readonly WarehouseDbContext _context;

    public CustomerOrderService(WarehouseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get order status by order ID
    /// </summary>
    public async Task<OrderStatusResponse?> GetOrderStatusAsync(int orderId)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.OrderedItems)
            .ThenInclude(oi => oi.Item)
            .Include(po => po.ReceivedShipments)
            .FirstOrDefaultAsync(po => po.PurchaseOrderId == orderId);

        if (purchaseOrder == null)
        {
            return null;
        }

        // Get the most recent shipment for tracking information
        var latestShipment = purchaseOrder.ReceivedShipments
            .OrderByDescending(s => s.DateReceived)
            .FirstOrDefault();

        return new OrderStatusResponse
        {
            OrderId = purchaseOrder.PurchaseOrderId,
            OrderNumber = purchaseOrder.OrderNumber ?? $"PO-{purchaseOrder.PurchaseOrderId}",
            Status = DetermineOrderStatus(purchaseOrder),
            OrderDate = purchaseOrder.OrderDate,
            CustomerName = purchaseOrder.Vendor?.VendorName,
            Items = purchaseOrder.OrderedItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ItemId,
                ProductName = oi.Item?.ItemName ?? "Unknown",
                QuantityOrdered = oi.QuantityOrdered,
                QuantityPicked = oi.IsPicked ? oi.QuantityOrdered : 0,
                UnitPrice = oi.PricePerUnit
            }).ToList(),
            TrackingNumber = latestShipment?.TrackingNumber,
            ShippingCarrier = latestShipment?.Carrier,
            ShippedDate = latestShipment?.DateReceived,
            EstimatedDeliveryDate = latestShipment?.DateReceived?.AddDays(7) // Example estimation
        };
    }

    private string DetermineOrderStatus(PurchaseOrder purchaseOrder)
    {
        // Determine status based on order state
        if (purchaseOrder.ReceivedShipments.Any(s => s.DateReceived.HasValue))
        {
            var allItemsReceived = purchaseOrder.OrderedItems.All(oi => 
                purchaseOrder.ReceivedShipments
                    .SelectMany(rs => rs.ReceivedItems)
                    .Where(ri => ri.ItemId == oi.ItemId)
                    .Sum(ri => ri.QuantityReceived) >= oi.QuantityOrdered
            );

            if (allItemsReceived)
            {
                return "Completed";
            }
            return "Partially Received";
        }

        if (purchaseOrder.OrderedItems.Any(oi => oi.IsPicked))
        {
            return "Picking";
        }

        return "Pending";
    }
}
