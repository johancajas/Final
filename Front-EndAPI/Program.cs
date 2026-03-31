using Front_EndAPI;
using Front_EndAPI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ============================================================
// AUTHENTICATION SERVICES REGISTRATION
// ============================================================
// Registers all authentication services needed for the app

// CustomAuthStateProvider: Stores JWT tokens in localStorage and manages user state
builder.Services.AddScoped<CustomAuthStateProvider>();

// Links Blazor's auth system to our custom provider
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// AuthService: Handles login/logout API calls
builder.Services.AddScoped<AuthService>();

// AuthHttpMessageHandler: Adds JWT token to all HTTP requests automatically
builder.Services.AddScoped<AuthHttpMessageHandler>();

// ============================================================
// HTTP CLIENT CONFIGURATION 
// ============================================================
// Configures HttpClient to automatically include JWT token in every request
builder.Services.AddScoped(sp =>
{
    // Get our custom handler that adds the token
    var handler = sp.GetRequiredService<AuthHttpMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();

    // Create HttpClient that talks to our backend API at localhost:7147
    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7147/")
    };
});

// ============================================================
// AUTHORIZATION POLICIES
// ============================================================
// Defines what permissions users need to access certain features
builder.Services.AddAuthorizationCore(options =>
{
    // "Create" policy: User must have "users.create" permission in their JWT token
    options.AddPolicy("Create", policy =>
        policy.RequireClaim("permission", "users.create"));
});

await builder.Build().RunAsync();

// ============================================================
// SUMMARY: How Auth Token Flows Through Your App
// ============================================================
// 1. USER LOGS IN:
//    - User enters credentials on login page
//    - AuthService sends credentials to backend API
//    - Backend returns JWT token if valid
//    - Token saved to browser's localStorage
//    - CustomAuthStateProvider updates app state to "authenticated"
//
// 2. MAKING API REQUESTS:
//    - User clicks button that needs data from API
//    - HttpClient starts to make request
//    - AuthHttpMessageHandler intercepts the request
//    - Handler gets token from CustomAuthStateProvider
//    - Adds "Authorization: Bearer {token}" header to request
//    - Request sent to backend with token
//    - Backend validates token and returns data
//
// 3. CHECKING PERMISSIONS:
//    - Component has [Authorize(Policy = "Create")] attribute
//    - Blazor asks AuthenticationStateProvider for user's claims
//    - CustomAuthStateProvider reads JWT token from localStorage
//    - Parses token to extract claims (userId, email, permissions)
//    - Blazor checks if user has required "users.create" claim
//    - Shows or hides component based on permission
//
// 4. USER LOGS OUT:
//    - User clicks logout button
//    - CustomAuthStateProvider removes token from localStorage
//    - App state updated to "anonymous"
//    - Future API requests have no Authorization header
//    - User redirected to login page
