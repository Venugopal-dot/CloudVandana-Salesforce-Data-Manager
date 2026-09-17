using CloudVandana.Api.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace CloudVandana.Api.Services;

public class SalesforceService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public SalesforceService(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccountsAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/query/" +
            "?q=SELECT+Id,Name,Phone,Website,Industry+FROM+Account";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));

        var response =
            await _httpClient.SendAsync(request);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return responseBody;
    }

    public async Task<string> GetAccountByIdAsync(string id)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken = session?.GetString("SalesforceAccessToken");

        var instanceUrl = session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url = $"{instanceUrl}/services/data/v66.0/sobjects/Account/{id}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return responseBody;
    }

    public async Task<string> CreateAccountAsync(CreateAccountRequest request)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/sobjects/Account";

        var accountData = new
        {
            Name = request.Name,
            Phone = request.Phone,
            Website = request.Website,
            Industry = request.Industry
        };

        var json = JsonSerializer.Serialize(accountData);

        using var content = new StringContent(
            json,
            System.Text.Encoding.UTF8,
            "application/json");

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            url);

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        httpRequest.Content = content;

        var response =
            await _httpClient.SendAsync(httpRequest);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return responseBody;
    }

    public async Task<string> UpdateAccountAsync(
    string id,
    UpdateAccountRequest request)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/sobjects/Account/{id}";

        var accountData = new
        {
            Name = request.Name,
            Phone = request.Phone,
            Website = request.Website,
            Industry = request.Industry
        };

        var json = JsonSerializer.Serialize(accountData);

        using var content = new StringContent(
            json,
            System.Text.Encoding.UTF8,
            "application/json");

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Patch,
            url);

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        httpRequest.Content = content;

        var response =
            await _httpClient.SendAsync(httpRequest);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return string.IsNullOrEmpty(responseBody)
            ? "{\"success\":true}"
            : responseBody;
    }

    public async Task<string> DeleteAccountAsync(string id)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/sobjects/Account/{id}";

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            url);

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        var response =
            await _httpClient.SendAsync(httpRequest);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return "{\"success\":true}";
    }

    public async Task<string> GetRecordsAsync(
    string objectName,
    int page = 1,
    int pageSize = 20)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var allowedObjects = new[]
        {
        "Account",
        "Opportunity",
        "Lead",
        "Contact",
        "Case"
    };

        if (!allowedObjects.Contains(
            objectName,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Unsupported Salesforce object.");
        }

        if (page < 1)
            page = 1;

        if (pageSize < 1 || pageSize > 20)
            pageSize = 20;

        var actualObjectName =
            allowedObjects.First(x =>
                x.Equals(
                    objectName,
                    StringComparison.OrdinalIgnoreCase));

        string fields = actualObjectName switch
        {
            "Account" =>
                "Id,Name,Phone,Website,Industry",

            "Opportunity" =>
                "Id,Name,Amount,StageName,CloseDate",

            "Lead" =>
                "Id,FirstName,LastName,Company,Email,Status",

            "Contact" =>
                "Id,FirstName,LastName,Email,Phone,Title",

            "Case" =>
                "Id,CaseNumber,Subject,Status,Priority",

            _ => "Id"
        };

        var offset = (page - 1) * pageSize;

        var soql =
            $"SELECT {fields} " +
            $"FROM {actualObjectName} " +
            $"LIMIT {pageSize} " +
            $"OFFSET {offset}";

        var url =
            $"{instanceUrl}/services/data/v66.0/query/" +
            $"?q={Uri.EscapeDataString(soql)}";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));

        var response =
            await _httpClient.SendAsync(request);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return responseBody;
    }
    public async Task<string> CreateRecordAsync(string objectName, Dictionary<string, object?> fields)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken = session?.GetString("SalesforceAccessToken");

        var instanceUrl = session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException("Salesforce authentication is required.");
        }

        var url = $"{instanceUrl}/services/data/v66.0/sobjects/{objectName}";

        using var request = new HttpRequestMessage(HttpMethod.Post,url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var json = JsonSerializer.Serialize(fields);

        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return responseBody;
    }

    public async Task<string> UpdateRecordAsync(
    string objectName,
    string id,
    Dictionary<string, object?> fields)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/sobjects/{objectName}/{id}";

        using var request =
            new HttpRequestMessage(HttpMethod.Patch, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        var json = JsonSerializer.Serialize(fields);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response =
            await _httpClient.SendAsync(request);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return string.IsNullOrEmpty(responseBody)
            ? "Record updated successfully."
            : responseBody;
    }

    public async Task<string> DeleteRecordAsync(
    string objectName,
    string id)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        var accessToken =
            session?.GetString("SalesforceAccessToken");

        var instanceUrl =
            session?.GetString("SalesforceInstanceUrl");

        if (string.IsNullOrEmpty(accessToken) ||
            string.IsNullOrEmpty(instanceUrl))
        {
            throw new UnauthorizedAccessException(
                "Salesforce authentication is required.");
        }

        var url =
            $"{instanceUrl}/services/data/v66.0/sobjects/{objectName}/{id}";

        using var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        using var response =
            await _httpClient.SendAsync(request);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Salesforce API error: {responseBody}");
        }

        return string.IsNullOrEmpty(responseBody)
            ? "Record deleted successfully."
            : responseBody;
    }
}