using Microsoft.AspNetCore.Mvc;

namespace CloudVandana.Api.Controllers;

[ApiController]
[Route("api/salesforce")]
public class SalesforceFieldController : ControllerBase
{
    [HttpGet("{objectName}/fields")]
    public IActionResult GetFields(string objectName)
    {
        var fields = objectName.ToLower() switch
        {
            "account" => new[]
            {
                "Id",
                "Name",
                "Phone",
                "Website",
                "Industry"
            },

            "opportunity" => new[]
            {
                "Id",
                "Name",
                "Amount",
                "StageName",
                "CloseDate"
            },

            "lead" => new[]
            {
                "Id",
                "FirstName",
                "LastName",
                "Company",
                "Email",
                "Status"
            },

            "contact" => new[]
            {
                "Id",
                "FirstName",
                "LastName",
                "Email",
                "Phone",
                "Title"
            },

            "case" => new[]
            {
                "Id",
                "CaseNumber",
                "Subject",
                "Status",
                "Priority"
            },

            _ => Array.Empty<string>()
        };

        if (fields.Length == 0)
        {
            return BadRequest("Unsupported Salesforce object.");
        }

        return Ok(fields);
    }
}