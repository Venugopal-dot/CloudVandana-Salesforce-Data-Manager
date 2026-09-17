using CloudVandana.Api.Models;
using CloudVandana.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudVandana.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly SalesforceService _salesforceService;

    public AccountController(SalesforceService salesforceService)
    {
        _salesforceService = salesforceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts =
            await _salesforceService.GetAccountsAsync();

        return Content(accounts, "application/json");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(string id)
    {
        var account = await _salesforceService.GetAccountByIdAsync(id);

        return Content(account, "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
    CreateAccountRequest request)
    {
        var result =
            await _salesforceService.CreateAccountAsync(request);

        return Content(
            result,
            "application/json");
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(
    string id,
    UpdateAccountRequest request)
    {
        var result =
            await _salesforceService.UpdateAccountAsync(
                id,
                request);

        return Content(
            result,
            "application/json");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var result =
            await _salesforceService.DeleteAccountAsync(id);

        return Content(
            result,
            "application/json");
    }

}