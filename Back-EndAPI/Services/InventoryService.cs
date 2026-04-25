using Back_EndAPI.Data;
using ClassLibrary.DTOs.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class InventoryService
{
    private readonly WarehouseDbContext _context;

    public InventoryService(WarehouseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get inventory report for all products or a specific product
    /// </summary>
    public async Task<InventoryReportResponse> GetInventoryReportAsync(int? productId = null)
    {
        var query = _context.Items.AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(i => i.ItemId == productId.Value);
        }

        var items = await query
            .Select(item => new InventoryItemDto
            {
                ProductId = item.ItemId,
                ProductName = item.ItemName ?? "Unknown",
                ProductDescription = item.Description,
                // Calculate quantities from related tables
                QuantityReceived = _context.ReceivedItems
                    .Where(ri => ri.ItemId == item.ItemId)
                    .Sum(ri => (int?)ri.QuantityReceived) ?? 0,
                QuantityPicked = _context.OrderedItems
                    .Where(oi => oi.ItemId == item.ItemId && oi.IsPicked)
                    .Sum(oi => (int?)oi.QuantityOrdered) ?? 0,
                TotalQuantityAvailable = (_context.ReceivedItems
                    .Where(ri => ri.ItemId == item.ItemId)
                    .Sum(ri => (int?)ri.QuantityReceived) ?? 0) -
                    (_context.OrderedItems
                    .Where(oi => oi.ItemId == item.ItemId && oi.IsPicked)
                    .Sum(oi => (int?)oi.QuantityOrdered) ?? 0)
            })
            .ToListAsync();

        return new InventoryReportResponse
        {
            Items = items
        };
    }
}
