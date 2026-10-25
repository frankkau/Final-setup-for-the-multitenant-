using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.LibraryBookCopy;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/library/book-copies")]
public class LibraryBookCopyController : ControllerBase
{
    private readonly ILibraryBookCopyService _service;

    public LibraryBookCopyController(
        ILibraryBookCopyService service)
    {
        _service = service;
    }


    [HttpGet("book/{libraryBookId:guid}")]
    public async Task<IActionResult> GetByBook(
        Guid libraryBookId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetByBookAsync(
                libraryBookId,
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


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLibraryBookCopyDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.LibraryBookCopyId },
            result);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateLibraryBookCopyDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}