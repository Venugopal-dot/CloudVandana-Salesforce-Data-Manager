namespace CloudVandana.Api.Models;

public class UpdateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
}