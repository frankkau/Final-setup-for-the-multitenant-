using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExpenseReceipt;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/expense-receipts")]
public class ExpenseReceiptController : ControllerBase
{
    private readonly IExpenseReceiptService _expenseReceiptService;

    public ExpenseReceiptController(
        IExpenseReceiptService expenseReceiptService)
    {
        _expenseReceiptService =
            expenseReceiptService;
    }


    [HttpGet("expense/{schoolExpenseId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<ExpenseReceiptDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByExpense(
        Guid schoolExpenseId,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseReceiptService.GetByExpenseAsync(
                schoolExpenseId,
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ExpenseReceiptDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseReceiptService.GetByIdAsync(
                id,
                cancellationToken);


        if (result is null)
            return NotFound(new
            {
                message = "Expense receipt not found."
            });


        return Ok(result);
    }


    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(
        typeof(ExpenseReceiptDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromForm] CreateExpenseReceiptDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseReceiptService.CreateAsync(
                dto,
                cancellationToken);


        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.ExpenseReceiptId
            },
            result);
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _expenseReceiptService.DeleteAsync(
            id,
            cancellationToken);


        return NoContent();
    }
}