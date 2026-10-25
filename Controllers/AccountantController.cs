using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Accountant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountantController : ControllerBase
{
    private readonly IAccountantService _accountantService;

    public AccountantController(
        IAccountantService accountantService)
    {
        _accountantService = accountantService;
    }

    // =========================================================
    // CREATE
    // POST: api/Accountant
    // =========================================================

    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<ActionResult<AccountantDto>> Create(
        [FromBody] CreateAccountantDto dto,
        CancellationToken cancellationToken)
    {
        var accountant =
            await _accountantService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = accountant.AccountantId },
            accountant);
    }

    // =========================================================
    // GET ALL
    // GET: api/Accountant
    // =========================================================

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountantDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var accountants =
            await _accountantService.GetAllAsync(
                cancellationToken);

        return Ok(accountants);
    }

    // =========================================================
    // GET BY ID
    // GET: api/Accountant/{id}
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountantDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var accountant =
            await _accountantService.GetByIdAsync(
                id,
                cancellationToken);

        if (accountant is null)
        {
            return NotFound(new
            {
                message = "Accountant not found."
            });
        }

        return Ok(accountant);
    }

    // =========================================================
    // GET BY EMPLOYEE NUMBER
    // GET: api/Accountant/employee/{employeeNo}
    // =========================================================

    [HttpGet("employee/{employeeNo}")]
    public async Task<ActionResult<AccountantDto>> GetByEmployeeNo(
        string employeeNo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(employeeNo))
        {
            return BadRequest(new
            {
                message = "Employee number is required."
            });
        }

        var accountant =
            await _accountantService.GetByEmployeeNoAsync(
                employeeNo,
                cancellationToken);

        if (accountant is null)
        {
            return NotFound(new
            {
                message = "Accountant not found."
            });
        }

        return Ok(accountant);
    }

    // =========================================================
    // UPDATE
    // PUT: api/Accountant/{id}
    // =========================================================

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<ActionResult<AccountantDto>> Update(
        Guid id,
        [FromBody] UpdateAccountantDto dto,
        CancellationToken cancellationToken)
    {
        var accountant =
            await _accountantService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(accountant);
    }

    // =========================================================
    // DELETE
    // DELETE: api/Accountant/{id}
    // =========================================================

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _accountantService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}