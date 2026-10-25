using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FeeStructure;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/fee-structures")]
public class FeeStructureController : ControllerBase
{
    private readonly IFeeStructureService _feeStructureService;

    public FeeStructureController(
        IFeeStructureService feeStructureService)
    {
        _feeStructureService = feeStructureService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<FeeStructureListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var feeStructures =
            await _feeStructureService.GetAllAsync(
                cancellationToken);

        return Ok(feeStructures);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(FeeStructureDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var feeStructure =
            await _feeStructureService.GetByIdAsync(
                id,
                cancellationToken);

        if (feeStructure is null)
        {
            return NotFound(new
            {
                message = "Fee structure not found."
            });
        }

        return Ok(feeStructure);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(FeeStructureDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateFeeStructureDto dto,
        CancellationToken cancellationToken)
    {
        var feeStructure =
            await _feeStructureService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = feeStructure.FeeStructureId
            },
            feeStructure);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(FeeStructureDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFeeStructureDto dto,
        CancellationToken cancellationToken)
    {
        var feeStructure =
            await _feeStructureService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(feeStructure);
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
        await _feeStructureService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}