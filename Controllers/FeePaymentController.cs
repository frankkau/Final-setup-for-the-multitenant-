using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.FeePayment;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/fee-payments")]
public class FeePaymentController : ControllerBase
{
    private readonly IFeePaymentService _feePaymentService;

    public FeePaymentController(
        IFeePaymentService feePaymentService)
    {
        _feePaymentService = feePaymentService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<FeePaymentListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var payments =
            await _feePaymentService.GetAllAsync(
                cancellationToken);

        return Ok(payments);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(FeePaymentDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var payment =
            await _feePaymentService.GetByIdAsync(
                id,
                cancellationToken);


        if (payment is null)
            return NotFound(new
            {
                message = "Fee payment not found."
            });


        return Ok(payment);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(FeePaymentDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateFeePaymentDto dto,
        CancellationToken cancellationToken)
    {
        var payment =
            await _feePaymentService.CreateAsync(
                dto,
                cancellationToken);


        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = payment.FeePaymentId
            },
            payment);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(FeePaymentDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFeePaymentDto dto,
        CancellationToken cancellationToken)
    {
        var payment =
            await _feePaymentService.UpdateAsync(
                id,
                dto,
                cancellationToken);


        return Ok(payment);
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
        await _feePaymentService.DeleteAsync(
            id,
            cancellationToken);


        return NoContent();
    }
}