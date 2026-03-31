using ClassLibrary.DTOs;
using Front_EndAPI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

public class IndexBase : ComponentBase
{
    // ============================================================
    // DEPENDENCY INJECTION
    // ============================================================
    // [Inject] tells Blazor to automatically provide these services when the component loads

    // HttpClient: Used to make API calls to the backend (already configured with auth token)
    [Inject]
    protected HttpClient Http { get; set; } = default!;

    // AuthStateProvider: Gives us access to authentication info (token, user claims)
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    // ============================================================
    // COMPONENT STATE - No explanation needed, just some properties to hold data and UI state
    // ============================================================
    protected List<CharacterDTO> Characters = new();
    protected string? Token { get; set; }
    protected CharacterDTO NewCharacter { get; set; } = new() { Level = 1, Health = 100, Mana = 50 };
    protected bool IsCreating { get; set; }
    protected bool IsLoading { get; set; }
    protected string CreateMessage { get; set; } = string.Empty;
    protected bool CreateSuccess { get; set; }

    // ============================================================
    // LIFECYCLE - No explanation needed, just load auth info and characters when the component initializes
    // ============================================================
    protected override async Task OnInitializedAsync()
    {
        await LoadAuthInfo();
        await LoadCharacters();
    }

    // ============================================================
    // AUTHENTICATION
    // ============================================================
    // Gets the current JWT token from the auth provider
    // Used to display the token on the page for learning purposes
    protected async Task LoadAuthInfo()
    {
        if (AuthStateProvider is CustomAuthStateProvider customProvider)
        {
            Token = customProvider.GetToken();
        }
        await Task.CompletedTask;
    }

    // ============================================================
    // API CALLS
    // ============================================================

    // GET request: Loads all characters from the backend
    // AuthHttpMessageHandler automatically adds the JWT token to this request
    protected async Task LoadCharacters()
    {
        IsLoading = true;
        try
        {
            Characters = await Http.GetFromJsonAsync<List<CharacterDTO>>("api/characters") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading characters: {ex.Message}");
            Characters = new();
        }
        finally
        {
            IsLoading = false;
        }
    }

    // POST request: Creates a new character by sending data to the backend
    // AuthHttpMessageHandler automatically adds the JWT token to this request
    protected async Task HandleCreateCharacter()
    {
        IsCreating = true;
        CreateMessage = string.Empty;

        try
        {
            var response = await Http.PostAsJsonAsync("api/characters", NewCharacter);

            if (response.IsSuccessStatusCode)
            {
                var createdCharacter = await response.Content.ReadFromJsonAsync<CharacterDTO>();
                CreateSuccess = true;
                CreateMessage = $"Character '{createdCharacter?.Name}' created successfully!";

                NewCharacter = new() { Level = 1, Health = 100, Mana = 50 };

                await LoadCharacters();
            }
            else
            {
                CreateSuccess = false;
                var errorContent = await response.Content.ReadAsStringAsync();
                CreateMessage = $"Failed to create character: {response.StatusCode}. {errorContent}";
            }
        }
        catch (Exception ex)
        {
            CreateSuccess = false;
            CreateMessage = $"Error creating character: {ex.Message}";
        }
        finally
        {
            IsCreating = false;
        }
    }
}