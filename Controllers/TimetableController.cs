using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos;
using CouncilRevenueCollection.Models.Dtos.Timetable;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/timetables")]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _timetableService;

    public TimetableController(
        ITimetableService timetableService)
    {
        _timetableService = timetableService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<TimetableListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result =
            await _timetableService.GetAllAsync(
                cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(TimetableDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result =
            await _timetableService.GetByIdAsync(
                id,
                cancellationToken);

        if (result is null)
            return NotFound(new
            {
                message = "Timetable not found."
            });

        return Ok(result);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(TimetableDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateTimetableDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _timetableService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.TimetableId
            },
            result);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(TimetableDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTimetableDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await _timetableService.UpdateAsync(
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
        await _timetableService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}