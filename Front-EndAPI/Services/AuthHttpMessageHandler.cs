using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace Front_EndAPI.Services;

// ============================================================
// AUTH HTTP MESSAGE HANDLER
// ============================================================
// Intercepts every HTTP request and automatically adds the JWT token to the Authorization header
// This is called a "DelegatingHandler" - it sits between your code and the actual HTTP request
public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthHttpMessageHandler(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    // This method runs before every HTTP request is sent
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get the token from CustomAuthStateProvider
        if (_authStateProvider is CustomAuthStateProvider customProvider)
        {
            var token = customProvider.GetToken();

            // If token exists, add it to the request header as "Authorization: Bearer {token}"
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Send the request with the token attached
        return await base.SendAsync(request, cancellationToken);
    }
}
