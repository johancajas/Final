using Back_EndAPI.Data;
using Back_EndAPI.Entities;
using ClassLibrary.DTOs.PurchaseOrder;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class PurchaseOrderService
{
    private readonly AppDbContext _context;

    public PurchaseOrderService(AppDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // CREATE PURCHASE ORDER
    // ============================================================
    public async Task<PurchaseOrderResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request)
    {
        // Validate supplier exists
        var supplier = await _context.StoreSuppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId);

        if (supplier == null)
        {
            throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found");
        }

        // Validate all items exist
        var itemIds = request.OrderItems.Select(oi => oi.ItemId).ToList();
        var items = await _context.StoreItems
            .Where(i => itemIds.Contains(i.Id))
            .ToListAsync();

        if (items.Count != itemIds.Count)
        {
            var missingIds = itemIds.Except(items.Select(i => i.Id));
            throw new InvalidOperationException($"Items not found: {string.Join(", ", missingIds)}");
        }

        // Create the purchase order
        var order = new StoreOrder
        {
            Supplierid = request.SupplierId,
            Datepurchased = request.DatePurchased
        };

        _context.StoreOrders.Add(order);
        await _context.SaveChangesAsync(); // Save to get the order ID

        // Create order items
        foreach (var itemRequest in request.OrderItems)
        {
            var orderItem = new StoreOrderItem
            {
                Orderid = order.Id,
                Itemid = itemRequest.ItemId,
                Quantity = itemRequest.Quantity,
                Actualprice = itemRequest.ActualPrice
            };

            _context.StoreOrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        // Return the created order with details
        return await GetPurchaseOrderByIdAsync(order.Id);
    }

    // ============================================================
    // GET PURCHASE ORDER BY ID
    // ============================================================
    public async Task<PurchaseOrderResponse> GetPurchaseOrderByIdAsync(int orderId)
    {
        var order = await _context.StoreOrders
            .Include(o => o.Supplier)
            .Include(o => o.StoreOrderItems)
                .ThenInclude(oi => oi.Item)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {orderId} not found");
        }

        var orderItems = order.StoreOrderItems.Select(oi => new OrderItemResponse
        {
            OrderItemId = oi.Id,
            ItemId = oi.Itemid ?? 0,
            ItemName = oi.Item?.Itemname,
            Quantity = oi.Quantity ?? 0,
            ActualPrice = oi.Actualprice ?? 0,
            LineTotal = (oi.Quantity ?? 0) * (oi.Actualprice ?? 0)
        }).ToList();

        return new PurchaseOrderResponse
        {
            OrderId = order.Id,
            SupplierId = order.Supplierid ?? 0,
            SupplierName = order.Supplier?.Suppliername,
            DatePurchased = order.Datepurchased ?? DateOnly.FromDateTime(DateTime.Now),
            OrderItems = orderItems,
            TotalAmount = orderItems.Sum(oi => oi.LineTotal)
        };
    }

    // ============================================================
    // GET ALL PURCHASE ORDERS
    // ============================================================
    public async Task<List<PurchaseOrderResponse>> GetAllPurchaseOrdersAsync()
    {
        var orders = await _context.StoreOrders
            .Include(o => o.Supplier)
            .Include(o => o.StoreOrderItems)
                .ThenInclude(oi => oi.Item)
            .OrderByDescending(o => o.Datepurchased)
            .ToListAsync();

        return orders.Select(order => new PurchaseOrderResponse
        {
            OrderId = order.Id,
            SupplierId = order.Supplierid ?? 0,
            SupplierName = order.Supplier?.Suppliername,
            DatePurchased = order.Datepurchased ?? DateOnly.FromDateTime(DateTime.Now),
            OrderItems = order.StoreOrderItems.Select(oi => new OrderItemResponse
            {
                OrderItemId = oi.Id,
                ItemId = oi.Itemid ?? 0,
                ItemName = oi.Item?.Itemname,
                Quantity = oi.Quantity ?? 0,
                ActualPrice = oi.Actualprice ?? 0,
                LineTotal = (oi.Quantity ?? 0) * (oi.Actualprice ?? 0)
            }).ToList(),
            TotalAmount = order.StoreOrderItems.Sum(oi => (oi.Quantity ?? 0) * (oi.Actualprice ?? 0))
        }).ToList();
    }
}
