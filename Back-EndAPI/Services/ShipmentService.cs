using Back_EndAPI.Data;
using Back_EndAPI.Entities.Warehouse;
using ClassLibrary.DTOs.Shipment;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class ShipmentService
{
    private readonly WarehouseDbContext _context;

    public ShipmentService(WarehouseDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // RECEIVE SHIPMENT
    // ============================================================
    /// Receives a shipment and updates inventory with strict validation
    public async Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request)
    {
        // Business Rule 1: Shipment must exist
        var shipment = await _context.ReceivedShipments
            .Include(s => s.PurchaseOrder)
                .ThenInclude(po => po!.OrderedItems)
                    .ThenInclude(oi => oi.SkuNumberNavigation)
            .Include(s => s.ReceivedItems)
            .FirstOrDefaultAsync(s => s.Id == request.ShipmentId);

        if (shipment == null)
        {
            throw new KeyNotFoundException($"Shipment with ID {request.ShipmentId} not found");
        }

        // Shipment must not already be received
        if (shipment.Status == "RECEIVED")
        {
            throw new InvalidOperationException($"Shipment {request.ShipmentId} has already been received and cannot be processed again");
        }

        // Validate purchase order exists
        if (shipment.PurchaseOrder == null)
        {
            throw new InvalidOperationException($"Shipment {request.ShipmentId} is not associated with a valid purchase order");
        }

        // Validate all ordered item IDs belong to this purchase order
        var orderedItemIds = shipment.PurchaseOrder.OrderedItems.Select(oi => oi.Id).ToList();
        var requestedItemIds = request.ReceivedItems.Select(ri => ri.OrderedItemId).ToList();

        var invalidIds = requestedItemIds.Except(orderedItemIds).ToList();
        if (invalidIds.Any())
        {
            throw new InvalidOperationException($"Invalid ordered item IDs: {string.Join(", ", invalidIds)}. These items do not belong to purchase order {shipment.PurchaseOrderId}");
        }

        // Validate quantities are positive
        var invalidQuantities = request.ReceivedItems.Where(ri => ri.ReceivedQuantity <= 0).ToList();
        if (invalidQuantities.Any())
        {
            throw new InvalidOperationException("All received quantities must be greater than 0");
        }

        var receivedDetails = new List<ReceivedItemDetail>();

        // Process each received item
        foreach (var receivedItemRequest in request.ReceivedItems)
        {
            var orderedItem = shipment.PurchaseOrder.OrderedItems
                .First(oi => oi.Id == receivedItemRequest.OrderedItemId);

            // Create or update received item record
            var receivedItem = new ReceivedItem
            {
                ShipmentId = shipment.Id,
                SkuNumber = orderedItem.SkuNumber,
                Qty = receivedItemRequest.ReceivedQuantity
            };

            _context.ReceivedItems.Add(receivedItem);

            // Calculate received vs ordered difference
            int orderedQty = orderedItem.Qty;
            int receivedQty = receivedItemRequest.ReceivedQuantity;
            int difference = receivedQty - orderedQty;

            string receivingStatus;
            if (difference == 0)
                receivingStatus = "Complete";
            else if (difference < 0)
                receivingStatus = $"Partial (Short by {Math.Abs(difference)})";
            else
                receivingStatus = $"Over (Extra {difference})";

            receivedDetails.Add(new ReceivedItemDetail
            {
                ReceivedItemId = receivedItem.Id,
                ProductId = orderedItem.SkuNumber ?? 0,
                ProductName = orderedItem.SkuNumberNavigation?.Name,
                OrderedQuantity = orderedQty,
                ReceivedQuantity = receivedQty,
                QuantityDifference = difference,
                ReceivingStatus = receivingStatus
            });
        }

        //  Update shipment status to RECEIVED
        shipment.Status = "RECEIVED";
        shipment.Date = request.ReceivedDate;

        // Update purchase order status based on received items
        await UpdatePurchaseOrderStatusAsync(shipment.PurchaseOrder.Id);

        await _context.SaveChangesAsync();

        return new ReceiveShipmentResponse
        {
            ShipmentId = shipment.Id,
            PurchaseOrderId = shipment.PurchaseOrderId ?? 0,
            ReceivedDate = request.ReceivedDate,
            Status = "RECEIVED",
            ReceivedItems = receivedDetails,
            Message = $"Shipment {shipment.Id} successfully received with {receivedDetails.Count} item types. Status updated to RECEIVED."
        };
    }

    // ============================================================
    // GET SHIPMENT BY ID
    // ============================================================
    public async Task<ReceiveShipmentResponse?> GetShipmentByIdAsync(int shipmentId)
    {
        var shipment = await _context.ReceivedShipments
            .Include(s => s.PurchaseOrder)
            .Include(s => s.ReceivedItems)
                .ThenInclude(ri => ri.SkuNumberNavigation)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment == null)
        {
            return null;
        }

        var items = shipment.ReceivedItems.Select(ri => new ReceivedItemDetail
        {
            ReceivedItemId = ri.Id,
            ProductId = ri.SkuNumber ?? 0,
            ProductName = ri.SkuNumberNavigation?.Name,
            OrderedQuantity = 0, 
            ReceivedQuantity = ri.Qty ?? 0,
            QuantityDifference = 0,
            ReceivingStatus = "Received"
        }).ToList();

        return new ReceiveShipmentResponse
        {
            ShipmentId = shipment.Id,
            PurchaseOrderId = shipment.PurchaseOrderId ?? 0,
            ReceivedDate = shipment.Date ?? DateOnly.FromDateTime(DateTime.Now),
            Status = shipment.Status,
            ReceivedItems = items,
            Message = $"Shipment {shipment.Id} - Status: {shipment.Status}"
        };
    }

    // ============================================================
    // GET ALL SHIPMENTS
    // ============================================================
    public async Task<List<ReceiveShipmentResponse>> GetAllShipmentsAsync()
    {
        var shipments = await _context.ReceivedShipments
            .Include(s => s.PurchaseOrder)
            .Include(s => s.ReceivedItems)
                .ThenInclude(ri => ri.SkuNumberNavigation)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        return shipments.Select(shipment => new ReceiveShipmentResponse
        {
            ShipmentId = shipment.Id,
            PurchaseOrderId = shipment.PurchaseOrderId ?? 0,
            ReceivedDate = shipment.Date ?? DateOnly.FromDateTime(DateTime.Now),
            Status = shipment.Status,
            ReceivedItems = shipment.ReceivedItems.Select(ri => new ReceivedItemDetail
            {
                ReceivedItemId = ri.Id,
                ProductId = ri.SkuNumber ?? 0,
                ProductName = ri.SkuNumberNavigation?.Name,
                OrderedQuantity = 0,
                ReceivedQuantity = ri.Qty ?? 0,
                QuantityDifference = 0,
                ReceivingStatus = "Received"
            }).ToList(),
            Message = $"Shipment {shipment.Id}"
        }).ToList();
    }

    // ============================================================
    // HELPER: UPDATE PURCHASE ORDER STATUS
    // ============================================================
    private async Task UpdatePurchaseOrderStatusAsync(int purchaseOrderId)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.ReceivedShipments)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrderId);

        if (purchaseOrder == null) return;

        // Check if all shipments are received
        var allShipmentsReceived = purchaseOrder.ReceivedShipments
            .All(s => s.Status == "RECEIVED");

        if (allShipmentsReceived && purchaseOrder.ReceivedShipments.Any())
        {
            purchaseOrder.Status = "RECEIVED";
        }
        else if (purchaseOrder.ReceivedShipments.Any(s => s.Status == "RECEIVED"))
        {
            purchaseOrder.Status = "PARTIALLY_RECEIVED";
        }

        await _context.SaveChangesAsync();
    }

    // ============================================================
    // GET INVENTORY REPORT
    // ============================================================
    public async Task<List<InventoryReportItem>> GetInventoryReportAsync()
    {
        // Get all items
        var items = await _context.Items.ToListAsync();
        var report = new List<InventoryReportItem>();

        foreach (var item in items)
        {
            // Calculate total received quantity
            var totalReceived = await _context.ReceivedItems
                .Where(ri => ri.SkuNumber == item.SkuNumber)
                .SumAsync(ri => ri.Qty ?? 0);

            report.Add(new InventoryReportItem
            {
                ProductId = item.SkuNumber,
                ProductName = item.Name,
                TotalReceivedQuantity = totalReceived,
                SuggestedSellingPrice = item.SuggestedSellingPrice ?? 0
            });
        }

        return report.OrderBy(r => r.ProductName).ToList();
    }
}

/// Inventory report item
public class InventoryReportItem
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int TotalReceivedQuantity { get; set; }
    public int SuggestedSellingPrice { get; set; }
}
