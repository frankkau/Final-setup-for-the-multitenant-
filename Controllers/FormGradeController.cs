using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FormGrade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FormGradesController : ControllerBase
{
    private readonly IFormGradeService _formGradeService;
    private readonly ILogger<FormGradesController> _logger;

    public FormGradesController(
        IFormGradeService formGradeService,
        ILogger<FormGradesController> logger)
    {
        _formGradeService = formGradeService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new Form/Grade.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FormGradeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FormGradeDto>> Create(
        [FromBody] CreateFormGradeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _formGradeService.CreateAsync(dto, cancellationToken);

        _logger.LogInformation(
            "FormGrade {FormGradeId} created successfully.",
            result.FormGradeId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.FormGradeId },
            result);
    }

    /// <summary>
    /// Gets a Form/Grade by Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FormGradeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormGradeDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _formGradeService.GetByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound(new
            {
                Message = "Form/Grade not found."
            });

        return Ok(result);
    }

    /// <summary>
    /// Gets all Form/Grades.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FormGradeListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FormGradeListDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _formGradeService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing Form/Grade.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FormGradeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FormGradeDto>> Update(
        Guid id,
        [FromBody] UpdateFormGradeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _formGradeService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a Form/Grade.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _formGradeService.DeleteAsync(
            id,
            cancellationToken);

        _logger.LogInformation(
            "FormGrade {FormGradeId} deleted successfully.",
            id);

        return NoContent();
    }
}