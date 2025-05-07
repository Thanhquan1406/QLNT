using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QLNT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceRepository invoiceRepository, ILogger<InvoiceController> logger)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
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
            try
            {
                // Lưu hóa đơn vào database
                var createdInvoice = await _invoiceRepository.AddAsync(invoice);
                return CreatedAtAction(nameof(GetInvoice), new { id = createdInvoice.InvoiceId }, createdInvoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn");
                return StatusCode(500, "Có lỗi xảy ra khi tạo hóa đơn");
            }
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
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInvoiceStatusRequest request)
        {
            try
            {
                var success = await _invoiceRepository.UpdateStatusAsync(id, request.Status);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái hóa đơn");
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật trạng thái hóa đơn");
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveInvoice(int id)
        {
            try
            {
                var success = await _invoiceRepository.ApproveInvoiceAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt hóa đơn");
                return StatusCode(500, "Có lỗi xảy ra khi duyệt hóa đơn");
            }
        }

        [HttpPut("{id}/payment")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentRequest request)
        {
            try
            {
                var success = await _invoiceRepository.UpdatePaymentAsync(id, request.PaidAmount);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thanh toán hóa đơn");
                return StatusCode(500, "Có lỗi xảy ra khi cập nhật thanh toán hóa đơn");
            }
        }

        [HttpGet("contract/{contractId}/debt")]
        public async Task<ActionResult<decimal>> GetContractTotalDebt(int contractId)
        {
            var totalDebt = await _invoiceRepository.GetTotalDebtByContractIdAsync(contractId);
            return Ok(totalDebt);
        }

        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetApprovedInvoices([FromQuery] bool isApproved = true)
        {
            var invoices = await _invoiceRepository.GetApprovedInvoicesAsync(isApproved);
            return Ok(invoices);
        }
    }

    public class UpdateInvoiceStatusRequest
    {
        public InvoiceStatus Status { get; set; }
    }

    public class UpdatePaymentRequest
    {
        public decimal PaidAmount { get; set; }
    }
} 