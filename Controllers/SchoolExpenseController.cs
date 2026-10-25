using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.SchoolExpense;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/school-expenses")]
public class SchoolExpenseController : ControllerBase
{
    private readonly ISchoolExpenseService _schoolExpenseService;

    public SchoolExpenseController(
        ISchoolExpenseService schoolExpenseService)
    {
        _schoolExpenseService = schoolExpenseService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<SchoolExpenseListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.GetAllAsync(
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(SchoolExpenseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
            return NotFound(new
            {
                message = "School expense not found."
            });

        return Ok(result);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(SchoolExpenseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateSchoolExpenseDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.SchoolExpenseId
            },
            result);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(SchoolExpenseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSchoolExpenseDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(result);
    }


    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _schoolExpenseService.ApproveAsync(
            id,
            cancellationToken);

        return NoContent();
    }


    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _schoolExpenseService.CancelAsync(
            id,
            cancellationToken);

        return NoContent();
    }


    [HttpGet("term/{termId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<SchoolExpenseListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTerm(
        Guid termId,
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.GetByTermAsync(
                termId,
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("academic-year/{academicYearId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<SchoolExpenseListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAcademicYear(
        Guid academicYearId,
        CancellationToken cancellationToken)
    {
        var result =
            await _schoolExpenseService.GetByAcademicYearAsync(
                academicYearId,
                cancellationToken);

        return Ok(result);
    }
}