using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Front_EndAPI.Services;

// ============================================================
// CUSTOM AUTH STATE PROVIDER
// ============================================================
// Manages authentication state for the entire Blazor app
// Stores JWT token and provides user information to components
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private string? _currentToken;

    // Returns current authentication state (logged in or anonymous)
    // Blazor calls this to check if user is authenticated
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrEmpty(_currentToken))
        {
            return Task.FromResult(new AuthenticationState(_anonymous));
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(_currentToken), "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    // ============================================================
    // CALLED WHEN USER LOGS IN
    // ============================================================
    // Saves the JWT token and tells the app the user is now authenticated
    // This triggers UI updates (shows protected content, hides login button, etc.)
    public void NotifyUserAuthentication(string token)
    {
        _currentToken = token;
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    // ============================================================
    // CALLED WHEN USER LOGS OUT
    // ============================================================
    // Removes the JWT token and tells the app the user is now logged out
    // This triggers UI updates (hides protected content, shows login button, etc.)
    public void NotifyUserLogout()
    {
        _currentToken = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    // ============================================================
    // EXTRACT USER INFO FROM JWT TOKEN
    // ============================================================
    // Decodes the JWT token to extract user information (userId, email, permissions)
    // Claims are key-value pairs stored inside the token
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }

    // ============================================================
    // PROVIDE TOKEN TO AuthHttpMessageHandler
    // ============================================================
    // Returns the stored token so AuthHttpMessageHandler can add it to API requests
    public string? GetToken() => _currentToken;
}
