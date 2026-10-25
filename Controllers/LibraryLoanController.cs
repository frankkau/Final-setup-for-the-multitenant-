using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.LibraryLoan;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/library/loans")]
public class LibraryLoanController : ControllerBase
{
    private readonly ILibraryLoanService _service;

    public LibraryLoanController(
        ILibraryLoanService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetAllAsync(
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByStudentAsync(
                studentId,
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }


    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow(
        CreateLibraryLoanDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.BorrowAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.LibraryLoanId },
            result);
    }


    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> Return(
        Guid id,
        decimal fineAmount,
        CancellationToken cancellationToken)
    {
        await _service.ReturnAsync(
            id,
            fineAmount,
            cancellationToken);

        return NoContent();
    }
}