using CloudVandana.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudVandana.Api.Controllers;

[ApiController]
[Route("api/salesforce")]
public class SalesforceObjectController : ControllerBase
{
    private readonly SalesforceService _salesforceService;

    public SalesforceObjectController(
        SalesforceService salesforceService)
    {
        _salesforceService = salesforceService;
    }

    [HttpGet("{objectName}")]
    public async Task<IActionResult> GetRecords(
    string objectName,
    int page = 1,
    int pageSize = 20)
    {
        var result = await _salesforceService.GetRecordsAsync(objectName, page, pageSize);

        return Content(result, "application/json");
    }

    [HttpPost("{objectName}")]
    public async Task<IActionResult> CreateRecord(
    string objectName,
    [FromBody] Dictionary<string, object?> fields)
    {
        var result = await _salesforceService.CreateRecordAsync(
            objectName,
            fields);

        return Ok(result);
    }

    [HttpPut("{objectName}/{id}")]
    public async Task<IActionResult> UpdateRecord(
    string objectName,
    string id,
    [FromBody] Dictionary<string, object?> fields)
    {
        var result = await _salesforceService.UpdateRecordAsync(
            objectName,
            id,
            fields);

        return Ok(result);
    }

    [HttpDelete("{objectName}/{id}")]
    public async Task<IActionResult> DeleteRecord(
    string objectName,
    string id)
    {
        var result = await _salesforceService.DeleteRecordAsync(
            objectName,
            id);

        return Ok(result);
    }
}