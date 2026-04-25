using Back_EndAPI.Data;
using Back_EndAPI.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Back_EndAPI.Services;

/// <summary>
/// Service for managing idempotent operations
/// </summary>
public class IdempotencyService
{
    private readonly WarehouseDbContext _context;

    public IdempotencyService(WarehouseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Check if a request with this idempotency key has already been processed
    /// </summary>
    public async Task<IdempotencyRecord?> GetExistingRecordAsync(string idempotencyKey, string operationName)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return null;

        return await _context.IdempotencyRecords
            .FirstOrDefaultAsync(r => 
                r.IdempotencyKey == idempotencyKey && 
                r.OperationName == operationName &&
                r.ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    /// Record a new idempotent operation
    /// </summary>
    public async Task<IdempotencyRecord> RecordOperationAsync(
        string idempotencyKey,
        string operationName,
        int statusCode,
        object responseBody,
        int? entityId = null,
        string? userId = null,
        int expirationHours = 24)
    {
        var record = new IdempotencyRecord
        {
            IdempotencyKey = idempotencyKey,
            OperationName = operationName,
            ProcessedAt = DateTime.UtcNow,
            StatusCode = statusCode,
            ResponseBody = JsonSerializer.Serialize(responseBody),
            EntityId = entityId,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };

        _context.IdempotencyRecords.Add(record);
        await _context.SaveChangesAsync();

        return record;
    }

    /// <summary>
    /// Deserialize the stored response body
    /// </summary>
    public T? DeserializeResponse<T>(IdempotencyRecord record)
    {
        if (string.IsNullOrWhiteSpace(record.ResponseBody))
            return default;

        return JsonSerializer.Deserialize<T>(record.ResponseBody);
    }

    /// <summary>
    /// Cleanup expired idempotency records
    /// </summary>
    public async Task CleanupExpiredRecordsAsync()
    {
        var expiredRecords = await _context.IdempotencyRecords
            .Where(r => r.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        _context.IdempotencyRecords.RemoveRange(expiredRecords);
        await _context.SaveChangesAsync();
    }
}
