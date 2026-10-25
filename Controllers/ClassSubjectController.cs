using CouncilRevenueCollection.Application.DTOs.ClassSubject;
using CouncilRevenueCollection.Application.Implementation;
using CouncilRevenueCollection.Application.Models.Dtos.ClassSubject;
using CouncilRevenueCollection.Models.Dtos.ClassSubject;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/class-subjects")]
public class ClassSubjectController : ControllerBase
{
    private readonly ClassSubjectService _service;


    public ClassSubjectController(
        ClassSubjectService service)
    {
        _service = service;
    }



    /// <summary>
    /// Get all class subjects
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ClassSubjectListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetAllAsync(cancellationToken);

        return Ok(result);
    }





    /// <summary>
    /// Get class subject by id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ClassSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .GetByIdAsync(
                id,
                cancellationToken);


        if (result is null)
        {
            return NotFound(new
            {
                message = "Class subject not found."
            });
        }


        return Ok(result);
    }





    /// <summary>
    /// Create class subject
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(ClassSubjectDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClassSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .CreateAsync(
                dto,
                cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.ClassSubjectId
            },
            result);
    }





    /// <summary>
    /// Update class subject
    /// </summary>
    [HttpPut("update/{id:guid}")]
    [ProducesResponseType(
        typeof(ClassSubjectDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateClassSubjectDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service
            .UpdateAsync(
                id,
                dto,
                cancellationToken);


        return Ok(result);
    }





    /// <summary>
    /// Delete class subject
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service
            .DeleteAsync(
                id,
                cancellationToken);


        return NoContent();
    }
}