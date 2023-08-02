using System.Security.Claims;
using System.Text.Json;

namespace GGR.Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ILocalStorageService _localStorageService;
    public CustomAuthStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwtToken = await _localStorageService.GetItemAsStringAsync("token");
        var identity = new ClaimsIdentity();

        if ( !string.IsNullOrEmpty(jwtToken) )
            identity = new ClaimsIdentity(ParseClaimsFromJwt(jwtToken), "jwt");

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch ( base64.Length % 4 )
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
