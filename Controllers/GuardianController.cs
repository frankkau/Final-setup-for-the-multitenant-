namespace CouncilRevenueCollection.Controllers;

using CouncilRevenueCollection.Application.Implementation;
using CouncilRevenueCollection.Models.Dtos.Guardian;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GuardiansController : ControllerBase
{
    private readonly IGuardianService _guardianService;
    private readonly ILogger<GuardiansController> _logger;


    public GuardiansController(
        IGuardianService guardianService,
        ILogger<GuardiansController> logger)
    {
        _guardianService = guardianService;
        _logger = logger;
    }



    // GET: api/Guardians
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GuardianDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GuardianDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var guardians = await _guardianService
            .GetAllAsync(cancellationToken);


        return Ok(guardians);
    }





    // GET: api/Guardians/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuardianDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var guardian = await _guardianService
            .GetByIdAsync(
                id,
                cancellationToken);



        if (guardian is null)
        {
            return NotFound(new
            {
                message = "Guardian not found."
            });
        }


        return Ok(guardian);
    }





    // POST: api/Guardians
    [HttpPost]
    [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuardianDto>> Create(
        [FromBody] CreateGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var guardian = await _guardianService
            .CreateAsync(
                dto,
                cancellationToken);



        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = guardian.GuardianId
            },
            guardian);
    }





    // PUT: api/Guardians/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuardianDto>> Update(
        Guid id,
        [FromBody] UpdateGuardianDto dto,
        CancellationToken cancellationToken)
    {
        var guardian = await _guardianService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);



        return Ok(guardian);
    }





    // DELETE: api/Guardians/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _guardianService
            .DeleteAsync(
                id,
                cancellationToken);



        _logger.LogInformation(
            "Guardian {GuardianId} deleted",
            id);



        return NoContent();
    }





    // HEAD: api/Guardians/{id}
    [HttpHead("{id:guid}")]
    public async Task<IActionResult> Exists(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exists = await _guardianService
            .ExistsAsync(
                id,
                cancellationToken);



        return exists
            ? Ok()
            : NotFound();
    }
}