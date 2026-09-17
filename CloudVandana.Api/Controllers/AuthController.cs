using System.Net.Http.Headers;
using System.Text.Json;
using CloudVandana.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CloudVandana.Api.Configuration;

namespace CloudVandana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SalesforceAuthService _authService;
    private readonly SalesforceSettings _settings;
    private readonly HttpClient _httpClient;

    public AuthController(
        SalesforceAuthService authService,
        IOptions<SalesforceSettings> settings)
    {
        _authService = authService;
        _settings = settings.Value;
        _httpClient = new HttpClient();
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var authorizationUrl =
            _authService.GetAuthorizationUrl(HttpContext);

        return Redirect(authorizationUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(
        string code,
        string state)
    {
        // 1. Validate state
        var savedState =
            HttpContext.Session.GetString("SalesforceState");

        if (string.IsNullOrEmpty(savedState) ||
            savedState != state)
        {
            return BadRequest("Invalid state.");
        }

        // 2. Get PKCE code verifier
        var codeVerifier =
            HttpContext.Session.GetString("SalesforceCodeVerifier");

        if (string.IsNullOrEmpty(codeVerifier))
        {
            return BadRequest("Code verifier not found.");
        }

        // 3. Prepare token request
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["redirect_uri"] = _settings.CallbackUrl,
            ["code_verifier"] = codeVerifier
        };

        using var content =
            new FormUrlEncodedContent(values);

        // 4. Exchange authorization code for tokens
        var response =
            await _httpClient.PostAsync(
                _settings.TokenUrl,
                content);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(responseBody);
        }

        // 5. Read Salesforce response
        using var json =
            JsonDocument.Parse(responseBody);

        var root = json.RootElement;

        var accessToken =
            root.GetProperty("access_token").GetString();

        var refreshToken =
            root.GetProperty("refresh_token").GetString();

        var instanceUrl =
            root.GetProperty("instance_url").GetString();

        // 6. Store tokens in server session
        HttpContext.Session.SetString(
            "SalesforceAccessToken",
            accessToken ?? "");

        HttpContext.Session.SetString(
            "SalesforceRefreshToken",
            refreshToken ?? "");

        HttpContext.Session.SetString(
            "SalesforceInstanceUrl",
            instanceUrl ?? "");

        return Redirect("https://localhost:4200/?login=success");
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var session = HttpContext.Session;

        var accessToken =
            session.GetString("SalesforceAccessToken");

        var instanceUrl =
            session.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            return Ok(new
            {
                isLoggedIn = false
            });
        }

        return Ok(new
        {
            isLoggedIn = true,
            instanceUrl
        });
    }
}