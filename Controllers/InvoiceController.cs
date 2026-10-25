using CouncilRevenueCollection.Application.InterfaceClass;
using CouncilRevenueCollection.Models.Dtos.Invoice;
using Microsoft.AspNetCore.Mvc;

namespace CouncilRevenueCollection.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(
        IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }


    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<InvoiceListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var invoices =
            await _invoiceService.GetAllAsync(
                cancellationToken);

        return Ok(invoices);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(InvoiceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceService.GetByIdAsync(
                id,
                cancellationToken);

        if (invoice is null)
        {
            return NotFound(new
            {
                message = "Invoice not found."
            });
        }

        return Ok(invoice);
    }


    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(
        typeof(IReadOnlyList<InvoiceListDto>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStudent(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var invoices =
            await _invoiceService.GetByStudentIdAsync(
                studentId,
                cancellationToken);

        return Ok(invoices);
    }


    [HttpPost]
    [ProducesResponseType(
        typeof(InvoiceDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceService.CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = invoice.InvoiceId
            },
            invoice);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(InvoiceDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        var invoice =
            await _invoiceService.UpdateAsync(
                id,
                dto,
                cancellationToken);

        return Ok(invoice);
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
        await _invoiceService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}