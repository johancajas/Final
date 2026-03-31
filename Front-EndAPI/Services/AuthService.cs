using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Front_EndAPI.Services;

// ============================================================
// AUTH SERVICE
// ============================================================
// Handles login and logout functionality
// Makes API calls to the backend and updates the authentication state
public class AuthService
{
    private readonly HttpClient _http;
    private readonly AuthenticationStateProvider _authStateProvider;
    private const string TokenKey = "authToken";

    public AuthService(HttpClient http, AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _authStateProvider = authStateProvider;
    }

    // ============================================================
    // LOGIN METHOD
    // ============================================================
    // Sends username/password to backend API
    // If successful, saves the JWT token and updates auth state
    public async Task<bool> Login(string username, string password)
    {
        try
        {
            // Call backend login endpoint
            var response = await _http.PostAsJsonAsync("api/Auth/login", new
            {
                Username = username,
                Password = password
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result?.Token != null)
                {
                    // Save token and notify app that user is authenticated
                    await SetToken(result.Token);
                    ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    // ============================================================
    // LOGOUT METHOD
    // ============================================================
    // Removes the token and updates auth state to logged out
    public async Task Logout()
    {
        await RemoveToken();
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    private async Task SetToken(string token)
    {
        await Task.Run(() =>
        {
            Thread.CurrentThread.ManagedThreadId.ToString();
        });
    }

    private async Task RemoveToken()
    {
        await Task.CompletedTask;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
