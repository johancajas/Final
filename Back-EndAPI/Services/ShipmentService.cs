using Back_EndAPI.Data;
using Back_EndAPI.Entities;
using ClassLibrary.DTOs.Shipment;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Services;

public class ShipmentService
{
    private readonly AppDbContext _context;

    public ShipmentService(AppDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // RECEIVE SHIPMENT AND UPDATE INVENTORY
    // ============================================================
    public async Task<ReceiveShipmentResponse> ReceiveShipmentAsync(ReceiveShipmentRequest request)
    {
        // 1. Validate purchase order exists
        var purchaseOrder = await _context.StoreOrders
            .Include(o => o.Supplier)
            .Include(o => o.StoreOrderItems)
                .ThenInclude(oi => oi.Item)
            .FirstOrDefaultAsync(o => o.Id == request.PurchaseOrderId);

        if (purchaseOrder == null)
        {
            throw new InvalidOperationException($"Purchase order {request.PurchaseOrderId} not found");
        }

        // 2. Validate all order item IDs exist in this purchase order
        var orderItemIds = purchaseOrder.StoreOrderItems.Select(oi => oi.Id).ToList();
        var requestedItemIds = request.ReceivedItems.Select(ri => ri.OrderItemId).ToList();

        var invalidIds = requestedItemIds.Except(orderItemIds).ToList();
        if (invalidIds.Any())
        {
            throw new InvalidOperationException($"Invalid order item IDs: {string.Join(", ", invalidIds)}. These items do not belong to order {request.PurchaseOrderId}");
        }

        // 3. Validate quantities are positive
        var invalidQuantities = request.ReceivedItems.Where(ri => ri.ReceivedQuantity <= 0).ToList();
        if (invalidQuantities.Any())
        {
            throw new InvalidOperationException("All received quantities must be greater than 0");
        }

        // 4. Create shipment receipt record
        var receipt = new ShipmentReceipt
        {
            Orderid = request.PurchaseOrderId,
            Receiveddate = request.ReceivedDate,
            Receivedby = "System", // Could be passed in request
            Notes = $"Received {request.ReceivedItems.Count} item types"
        };

        _context.ShipmentReceipts.Add(receipt);
        await _context.SaveChangesAsync(); // Save to get receipt ID

        var receivedDetails = new List<ReceivedItemDetail>();

        // 5. Process each received item
        foreach (var receivedItem in request.ReceivedItems)
        {
            var orderItem = purchaseOrder.StoreOrderItems.First(oi => oi.Id == receivedItem.OrderItemId);

            // Create receipt item record
            var receiptItem = new ShipmentReceiptItem
            {
                Receiptid = receipt.Id,
                Orderitemid = receivedItem.OrderItemId,
                Receivedquantity = receivedItem.ReceivedQuantity
            };
            _context.ShipmentReceiptItems.Add(receiptItem);

            // Create inventory transaction
            var transaction = new InventoryTransaction
            {
                Itemid = orderItem.Itemid,
                Transactiontype = "RECEIVE",
                Quantity = receivedItem.ReceivedQuantity,
                Transactiondate = request.ReceivedDate,
                Referenceid = receipt.Id,
                Notes = $"Received from PO#{request.PurchaseOrderId}"
            };
            _context.InventoryTransactions.Add(transaction);

            // Calculate new inventory count
            var currentInventory = await GetCurrentInventoryCountAsync(orderItem.Itemid ?? 0);
            var newInventory = currentInventory + receivedItem.ReceivedQuantity;

            // Compare received vs ordered
            int orderedQty = orderItem.Quantity ?? 0;
            int difference = receivedItem.ReceivedQuantity - orderedQty;
            string status;

            if (difference == 0)
                status = "Complete";
            else if (difference < 0)
                status = $"Partial (Short by {Math.Abs(difference)})";
            else
                status = $"Over (Extra {difference})";

            receivedDetails.Add(new ReceivedItemDetail
            {
                ItemId = orderItem.Itemid ?? 0,
                ItemName = orderItem.Item?.Itemname,
                OrderedQuantity = orderedQty,
                ReceivedQuantity = receivedItem.ReceivedQuantity,
                QuantityDifference = difference,
                ActualPrice = orderItem.Actualprice ?? 0,
                NewInventoryCount = newInventory,
                Status = status
            });
        }

        await _context.SaveChangesAsync();

        // 6. Return response
        return new ReceiveShipmentResponse
        {
            PurchaseOrderId = request.PurchaseOrderId,
            OrderDate = purchaseOrder.Datepurchased ?? DateOnly.FromDateTime(DateTime.Now),
            ReceivedDate = request.ReceivedDate,
            SupplierName = purchaseOrder.Supplier?.Suppliername,
            ReceivedItems = receivedDetails,
            Message = $"Successfully received {receivedDetails.Count} item types. Inventory updated."
        };
    }

    // ============================================================
    // GET CURRENT INVENTORY COUNT FOR AN ITEM
    // ============================================================
    private async Task<int> GetCurrentInventoryCountAsync(int itemId)
    {
        // Sum all RECEIVE transactions and subtract all SELL transactions
        var transactions = await _context.InventoryTransactions
            .Where(t => t.Itemid == itemId)
            .ToListAsync();

        int totalReceived = transactions
            .Where(t => t.Transactiontype == "RECEIVE")
            .Sum(t => t.Quantity ?? 0);

        int totalSold = transactions
            .Where(t => t.Transactiontype == "SELL")
            .Sum(t => t.Quantity ?? 0);

        int adjustments = transactions
            .Where(t => t.Transactiontype == "ADJUST")
            .Sum(t => t.Quantity ?? 0);

        return totalReceived - totalSold + adjustments;
    }

    // ============================================================
    // GET INVENTORY REPORT
    // ============================================================
    public async Task<List<InventoryReportItem>> GetInventoryReportAsync()
    {
        var items = await _context.StoreItems.ToListAsync();
        var report = new List<InventoryReportItem>();

        foreach (var item in items)
        {
            var count = await GetCurrentInventoryCountAsync(item.Id);
            report.Add(new InventoryReportItem
            {
                ItemId = item.Id,
                ItemName = item.Itemname,
                CurrentStock = count,
                ShelfPrice = item.Shelfprice ?? 0
            });
        }

        return report.OrderBy(r => r.ItemName).ToList();
    }
}

public class InventoryReportItem
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int CurrentStock { get; set; }
    public decimal ShelfPrice { get; set; }
}
