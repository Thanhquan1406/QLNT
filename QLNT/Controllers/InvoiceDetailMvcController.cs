using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNT.Controllers
{
    public class InvoiceDetailMvcController : Controller
    {
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceDetailMvcController(IInvoiceDetailRepository invoiceDetailRepository, IInvoiceRepository invoiceRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceRepository = invoiceRepository;
        }

        // GET: InvoiceDetailMvc
        public async Task<IActionResult> Index()
        {
            var details = await _invoiceDetailRepository.GetAllAsync();
            return View(details);
        }

        // GET: InvoiceDetailMvc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var detail = await _invoiceDetailRepository.GetByIdAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            return View(detail);
        }

        // GET: InvoiceDetailMvc/Create
        public async Task<IActionResult> Create()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.Invoices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(invoices, "InvoiceId", "InvoiceId");
            return View();
        }

        // POST: InvoiceDetailMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceDetail detail)
        {
            if (ModelState.IsValid)
            {
                await _invoiceDetailRepository.AddAsync(detail);
                return RedirectToAction(nameof(Index));
            }
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.Invoices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(invoices, "InvoiceId", "InvoiceId");
            return View(detail);
        }

        // GET: InvoiceDetailMvc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var detail = await _invoiceDetailRepository.GetByIdAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.Invoices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(invoices, "InvoiceId", "InvoiceId");
            return View(detail);
        }

        // POST: InvoiceDetailMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceDetail detail)
        {
            if (id != detail.InvoiceDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _invoiceDetailRepository.UpdateAsync(detail);
                return RedirectToAction(nameof(Index));
            }
            var invoices = await _invoiceRepository.GetAllAsync();
            ViewBag.Invoices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(invoices, "InvoiceId", "InvoiceId");
            return View(detail);
        }

        // GET: InvoiceDetailMvc/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _invoiceDetailRepository.GetByIdAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            return View(detail);
        }

        // POST: InvoiceDetailMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceDetailRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 