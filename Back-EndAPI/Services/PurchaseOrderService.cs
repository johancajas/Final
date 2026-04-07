using Back_EndAPI.Data;
using Back_EndAPI.Entities.Warehouse;
using ClassLibrary.DTOs.PurchaseOrder;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class PurchaseOrderService
{
    private readonly WarehouseDbContext _context;

    public PurchaseOrderService(WarehouseDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // CREATE PURCHASE ORDER
    // ============================================================
    /// <summary>
    /// Creates a new purchase order with proper validation and business rules
    /// </summary>
    public async Task<PurchaseOrderResponse> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request)
    {
        // Business Rule 1: Validate vendor exists
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == request.VendorId);

        if (vendor == null)
        {
            throw new InvalidOperationException($"Vendor with ID {request.VendorId} not found");
        }

        // Business Rule 2: Validate all product IDs exist
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _context.Items
            .Where(i => productIds.Contains(i.SkuNumber))
            .Select(i => i.SkuNumber)
            .ToListAsync();

        var missingProducts = productIds.Except(existingProducts).ToList();
        if (missingProducts.Any())
        {
            throw new InvalidOperationException($"Products not found: {string.Join(", ", missingProducts)}");
        }

        // Business Rule 3: Validate no duplicate products in the order
        var duplicates = request.Items
            .GroupBy(i => i.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Any())
        {
            throw new InvalidOperationException($"Duplicate product IDs found: {string.Join(", ", duplicates)}. Each product can only appear once per order.");
        }

        // Business Rule 4: Calculate expected total cost
        decimal expectedTotalCost = request.Items.Sum(i => i.Quantity * i.CostPerUnit);

        // Create the purchase order with status CREATED
        var purchaseOrder = new PurchaseOrder
        {
            DateOrdered = request.DateOrdered,
            Vendorid = request.VendorId,
            ExpectedTotalCost = (int)expectedTotalCost, // Assuming integer cents
            Status = "CREATED" // Business Rule: Initial status is CREATED
        };

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync(); // Save to get the order ID

        // Create ordered items
        foreach (var itemRequest in request.Items)
        {
            var orderedItem = new OrderedItem
            {
                PurchaseId = purchaseOrder.Id,
                SkuNumber = itemRequest.ProductId,
                Qty = itemRequest.Quantity,
                CostPerUnit = itemRequest.CostPerUnit
            };

            _context.OrderedItems.Add(orderedItem);
        }

        await _context.SaveChangesAsync();

        // Return the created order with full details
        return await GetPurchaseOrderByIdAsync(purchaseOrder.Id);
    }

    // ============================================================
    // GET PURCHASE ORDER BY ID
    // ============================================================
    public async Task<PurchaseOrderResponse> GetPurchaseOrderByIdAsync(int orderId)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Vendor)
            .Include(o => o.OrderedItems)
                .ThenInclude(oi => oi.SkuNumberNavigation)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Purchase order {orderId} not found");
        }

        var items = order.OrderedItems.Select(oi => new OrderedItemResponse
        {
            OrderedItemId = oi.Id,
            ProductId = oi.SkuNumber ?? 0,
            ProductName = oi.SkuNumberNavigation?.Name,
            Quantity = oi.Qty,
            CostPerUnit = oi.CostPerUnit,
            LineTotal = oi.Qty * oi.CostPerUnit
        }).ToList();

        return new PurchaseOrderResponse
        {
            OrderId = order.Id,
            VendorId = order.Vendorid ?? 0,
            VendorName = order.Vendor?.Name,
            DateOrdered = order.DateOrdered,
            Status = order.Status,
            Items = items,
            ExpectedTotalCost = items.Sum(i => i.LineTotal)
        };
    }

    // ============================================================
    // GET ALL PURCHASE ORDERS
    // ============================================================
    public async Task<List<PurchaseOrderResponse>> GetAllPurchaseOrdersAsync()
    {
        var orders = await _context.PurchaseOrders
            .Include(o => o.Vendor)
            .Include(o => o.OrderedItems)
                .ThenInclude(oi => oi.SkuNumberNavigation)
            .OrderByDescending(o => o.DateOrdered)
            .ToListAsync();

        return orders.Select(order => new PurchaseOrderResponse
        {
            OrderId = order.Id,
            VendorId = order.Vendorid ?? 0,
            VendorName = order.Vendor?.Name,
            DateOrdered = order.DateOrdered,
            Status = order.Status,
            Items = order.OrderedItems.Select(oi => new OrderedItemResponse
            {
                OrderedItemId = oi.Id,
                ProductId = oi.SkuNumber ?? 0,
                ProductName = oi.SkuNumberNavigation?.Name,
                Quantity = oi.Qty,
                CostPerUnit = oi.CostPerUnit,
                LineTotal = oi.Qty * oi.CostPerUnit
            }).ToList(),
            ExpectedTotalCost = order.OrderedItems.Sum(oi => oi.Qty * oi.CostPerUnit)
        }).ToList();
    }

    // ============================================================
    // UPDATE PURCHASE ORDER STATUS
    // ============================================================
    public async Task UpdatePurchaseOrderStatusAsync(int orderId, string newStatus)
    {
        var order = await _context.PurchaseOrders.FindAsync(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Purchase order {orderId} not found");
        }

        // Validate status transition (you can add more rules here)
        var validStatuses = new[] { "CREATED", "APPROVED", "ORDERED", "PARTIALLY_RECEIVED", "RECEIVED", "CANCELLED" };
        if (!validStatuses.Contains(newStatus))
        {
            throw new InvalidOperationException($"Invalid status: {newStatus}");
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();
    }
}
