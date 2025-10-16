using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace HealthApp.Blazor.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;
    private readonly AuthenticationState _anonymous;
    private const string TOKEN_KEY = "authToken";

    public CustomAuthStateProvider(IJSRuntime js, HttpClient http)
    {
        _js = js;
        _http = http;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        try
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            return _anonymous;
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var claims = keyValuePairs.Select(kvp =>
        {
            var type = kvp.Key switch
            {
                "role" => ClaimTypes.Role,              // JWT role
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" => ClaimTypes.Role,
                "sub" => ClaimTypes.Email,             // User.Identity.Name
                _ => kvp.Key
            };
            return new Claim(type, kvp.Value?.ToString() ?? "");
        });

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
