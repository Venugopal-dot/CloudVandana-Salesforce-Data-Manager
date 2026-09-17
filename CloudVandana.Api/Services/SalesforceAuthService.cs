using System.Security.Cryptography;
using CloudVandana.Api.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CloudVandana.Api.Services;

public class SalesforceAuthService
{
    private readonly SalesforceSettings _settings;

    public SalesforceAuthService(IOptions<SalesforceSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GetAuthorizationUrl(HttpContext httpContext)
    {
        // Create PKCE code verifier
        var verifierBytes = RandomNumberGenerator.GetBytes(32);
        var codeVerifier = WebEncoders.Base64UrlEncode(verifierBytes);

        // Create PKCE code challenge
        var challengeBytes =
            SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(codeVerifier));

        var codeChallenge =
            WebEncoders.Base64UrlEncode(challengeBytes);

        // Create state value
        var stateBytes = RandomNumberGenerator.GetBytes(32);
        var state = WebEncoders.Base64UrlEncode(stateBytes);

        // Store values in session for the callback
        httpContext.Session.SetString("SalesforceCodeVerifier", codeVerifier);
        httpContext.Session.SetString("SalesforceState", state);

        var url =
            $"{_settings.AuthorizationUrl}" +
            $"?response_type=code" +
            $"&client_id={Uri.EscapeDataString(_settings.ClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_settings.CallbackUrl)}" +
            $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
            $"&code_challenge_method=S256" +
            $"&state={Uri.EscapeDataString(state)}";

        return url;
    }
}