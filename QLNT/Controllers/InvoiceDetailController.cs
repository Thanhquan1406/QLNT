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
    public class InvoiceDetailController : ControllerBase
    {
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;

        public InvoiceDetailController(IInvoiceDetailRepository invoiceDetailRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDetail>>> GetAllInvoiceDetails()
        {
            var details = await _invoiceDetailRepository.GetAllAsync();
            return Ok(details);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetail>> GetInvoiceDetail(int id)
        {
            var detail = await _invoiceDetailRepository.GetByIdAsync(id);
            if (detail == null)
                return NotFound();
            return Ok(detail);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDetail>> CreateInvoiceDetail(InvoiceDetail detail)
        {
            var createdDetail = await _invoiceDetailRepository.AddAsync(detail);
            return CreatedAtAction(nameof(GetInvoiceDetail), new { id = createdDetail.InvoiceDetailId }, createdDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoiceDetail(int id, InvoiceDetail detail)
        {
            if (id != detail.InvoiceDetailId)
                return BadRequest();

            var updatedDetail = await _invoiceDetailRepository.UpdateAsync(detail);
            return Ok(updatedDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetail(int id)
        {
            var result = await _invoiceDetailRepository.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDetail>>> GetDetailsByInvoice(int invoiceId)
        {
            var details = await _invoiceDetailRepository.GetByInvoiceIdAsync(invoiceId);
            return Ok(details);
        }

        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDetail>>> GetDetailsByService(int serviceId)
        {
            var details = await _invoiceDetailRepository.GetByServiceIdAsync(serviceId);
            return Ok(details);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<InvoiceDetail>>> GetDetailsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var details = await _invoiceDetailRepository.GetByDateRangeAsync(startDate, endDate);
            return Ok(details);
        }

        [HttpGet("invoice/{invoiceId}/total")]
        public async Task<ActionResult<decimal>> GetTotalAmountByInvoice(int invoiceId)
        {
            var total = await _invoiceDetailRepository.GetTotalAmountByInvoiceIdAsync(invoiceId);
            return Ok(total);
        }
    }
} 