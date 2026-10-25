using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.ExpenseCategory;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/expense-categories")]
public class ExpenseCategoryController : ControllerBase
{
    private readonly IExpenseCategoryService _expenseCategoryService;

    public ExpenseCategoryController(
        IExpenseCategoryService expenseCategoryService)
    {
        _expenseCategoryService = expenseCategoryService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ExpenseCategoryDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseCategoryService.GetAllAsync(
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ExpenseCategoryDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseCategoryService.GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
            return NotFound(new
            {
                message = "Expense category not found."
            });

        return Ok(result);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(ExpenseCategoryDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateExpenseCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseCategoryService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.ExpenseCategoryId
            },
            result);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(ExpenseCategoryDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateExpenseCategoryDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _expenseCategoryService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(result);
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
        await _expenseCategoryService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}