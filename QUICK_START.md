# Quick Start Guide - Reporting Endpoints

## Step 1: Fix the Port Issue

Before you can run the application, you need to resolve the port 5285 conflict.

### Option A: Change the Port (Recommended)
1. Open `Back-EndAPI/Properties/launchSettings.json`
2. Change port 5285 to 5286 (or any available port):
```json
{
  "profiles": {
	"http": {
	  "applicationUrl": "http://localhost:5286"
	},
	"https": {
	  "applicationUrl": "https://localhost:7147;http://localhost:5286"
	}
  }
}
```

### Option B: Kill the Process Using Port 5285
1. Open Command Prompt or PowerShell as Administrator
2. Run: `netstat -ano | findstr :5285`
3. Note the PID (Process ID) from the output
4. Run: `taskkill /PID <PID> /F` (replace `<PID>` with the actual number)

## Step 2: Build the Solution

```bash
dotnet build
```

## Step 3: Run the Application

Press F5 in Visual Studio or run:
```bash
dotnet run --project Back-EndAPI
```

## Step 4: Test the Endpoints

### Using Swagger UI
1. Navigate to `http://localhost:5286` (or your configured port)
2. You'll see the Swagger UI with all endpoints
3. Expand the `/api/inventory` and `/api/orders/{id}` endpoints
4. Click "Try it out" and test them

### Using the HTTP File
1. Open `Back-EndAPI/ReportingEndpoints.http` in Visual Studio
2. Update the port if you changed it from 5285
3. Click the "Send Request" link above each request

### Using cURL

**Get all inventory:**
```bash
curl http://localhost:5286/api/inventory
```

**Get specific product inventory:**
```bash
curl http://localhost:5286/api/inventory?productId=1
```

**Get order status:**
```bash
curl http://localhost:5286/api/orders/1
```

## Step 5: Verify Entity Compatibility

If you encounter errors about missing properties or tables, you may need to update the service implementations to match your actual database schema.

Check these files:
- `Back-EndAPI/Services/InventoryService.cs`
- `Back-EndAPI/Services/CustomerOrderService.cs`

Ensure the property names match your entity models in `Back-EndAPI/Entities/Warehouse/`

## What You Should See

### Successful Inventory Response (200 OK):
```json
{
  "items": [
	{
	  "productId": 1,
	  "productName": "Example Product",
	  "productDescription": "Product description",
	  "totalQuantityAvailable": 50,
	  "quantityReceived": 100,
	  "quantityPicked": 50
	}
  ]
}
```

### Successful Order Response (200 OK):
```json
{
  "orderId": 1,
  "orderNumber": "PO-001",
  "status": "Completed",
  "orderDate": "2024-01-15T10:30:00Z",
  "customerName": "Vendor Name",
  "items": [
	{
	  "productId": 1,
	  "productName": "Product Name",
	  "quantityOrdered": 10,
	  "quantityPicked": 10,
	  "unitPrice": 25.99
	}
  ],
  "trackingNumber": "1Z999AA10123456784",
  "shippingCarrier": "UPS",
  "shippedDate": "2024-01-16T14:00:00Z",
  "estimatedDeliveryDate": "2024-01-23T14:00:00Z"
}
```

### Not Found Response (404):
```json
{
  "message": "Product with ID 99999 not found."
}
```

## Troubleshooting

### Error: "Table or view not found"
- Your entity names might be different from what's expected
- Check the entity models in `Back-EndAPI/Entities/Warehouse/`
- Update the service implementations to use the correct table/property names

### Error: "Property does not exist"
- Check your entity property names
- Update the LINQ queries in the service files to match your entities

### Empty Results
- Ensure your database has data
- Check that the database connection string in `appsettings.json` is correct
- Verify the database tables have the expected structure

## Next Steps

1. ✅ Port issue resolved
2. ✅ Application running
3. ✅ Endpoints tested and working
4. Consider adding authentication with `[Authorize]` attributes if needed
5. Consider adding logging for production monitoring
6. Write integration tests for the endpoints

## Need Help?

Refer to these documentation files:
- `IMPLEMENTATION_SUMMARY.md` - Overview of what was implemented
- `REPORTING_ENDPOINTS_README.md` - Detailed endpoint documentation
