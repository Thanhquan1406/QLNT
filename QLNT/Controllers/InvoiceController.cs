using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
        {
            var createdInvoice = await _invoiceRepository.AddAsync(invoice);
            return CreatedAtAction(nameof(GetInvoice), new { id = createdInvoice.InvoiceId }, createdInvoice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
                return BadRequest();

            var updatedInvoice = await _invoiceRepository.UpdateAsync(invoice);
            return Ok(updatedInvoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceRepository.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByContract(int contractId)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            return Ok(invoices);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByCustomer(int customerId)
        {
            var invoices = await _invoiceRepository.GetByCustomerIdAsync(customerId);
            return Ok(invoices);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByStatus(InvoiceStatus status)
        {
            var invoices = await _invoiceRepository.GetByStatusAsync(status);
            return Ok(invoices);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            return Ok(invoices);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] InvoiceStatusUpdateDto statusUpdate)
        {
            var result = await _invoiceRepository.UpdateStatusAsync(id, statusUpdate.Status, statusUpdate.PaidDate);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }

    public class InvoiceStatusUpdateDto
    {
        public InvoiceStatus Status { get; set; }
        public DateTime? PaidDate { get; set; }
    }
} 