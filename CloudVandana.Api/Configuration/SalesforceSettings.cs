namespace CloudVandana.Api.Configuration;

public class SalesforceSettings
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string CallbackUrl { get; set; } = string.Empty;

    public string AuthorizationUrl { get; set; } = string.Empty;

    public string TokenUrl { get; set; } = string.Empty;
}