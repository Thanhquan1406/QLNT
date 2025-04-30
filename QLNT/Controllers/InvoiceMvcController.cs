using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNT.Controllers
{
    public class InvoiceMvcController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<InvoiceMvcController> _logger;

        public InvoiceMvcController(
            IInvoiceRepository invoiceRepository, 
            IContractRepository contractRepository,
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository,
            ILogger<InvoiceMvcController> logger)
        {
            _invoiceRepository = invoiceRepository;
            _contractRepository = contractRepository;
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
            _logger = logger;
        }

        // GET: InvoiceMvc
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return View(invoices);
        }

        // GET: InvoiceMvc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // GET: InvoiceMvc/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Bắt đầu tạo hóa đơn mới");
            try
            {
                var buildings = await _buildingRepository.GetAllBuildingsAsync();
                var rooms = await _roomRepository.GetAllAsync();
                var contracts = await _contractRepository.GetAllAsync();

                ViewBag.Buildings = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(buildings, "Id", "Name");
                ViewBag.Rooms = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(rooms, "Id", "RoomNumber");
                ViewBag.Contracts = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(contracts, "ContractId", "ContractNumber");
                
                _logger.LogInformation("Đã tải dữ liệu cho form tạo hóa đơn");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dữ liệu cho form tạo hóa đơn");
                throw;
            }
        }

        // POST: InvoiceMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            _logger.LogInformation("Bắt đầu tạo hóa đơn mới");
            _logger.LogInformation("Dữ liệu hóa đơn nhận được từ form:");
            _logger.LogInformation("- ContractId: {ContractId}", invoice.ContractId);
            _logger.LogInformation("- InvoiceType: {InvoiceType}", invoice.InvoiceType);
            _logger.LogInformation("- Period: {Period}", invoice.Period);
            _logger.LogInformation("- IssueDate: {IssueDate}", invoice.IssueDate);
            _logger.LogInformation("- DueDate: {DueDate}", invoice.DueDate);
            _logger.LogInformation("- Status: {Status}", invoice.Status);
            _logger.LogInformation("- PaymentMethod: {PaymentMethod}", invoice.PaymentMethod);
            _logger.LogInformation("- ReferenceNumber: {ReferenceNumber}", invoice.ReferenceNumber);
            _logger.LogInformation("- Notes: {Notes}", invoice.Notes);

            // Kiểm tra hợp đồng tồn tại trước khi validate
            _logger.LogInformation("Đang tìm hợp đồng với ID: {ContractId}", invoice.ContractId);
            var contract = await _contractRepository.GetByIdAsync(invoice.ContractId);
            
            if (contract == null)
            {
                _logger.LogWarning("Không tìm thấy hợp đồng với ID: {ContractId}", invoice.ContractId);
                ModelState.AddModelError("", "Hợp đồng không tồn tại");
                return View(invoice);
            }

            _logger.LogInformation("Thông tin hợp đồng tìm được:");
            _logger.LogInformation("- ContractId: {ContractId}", contract.Id);
            _logger.LogInformation("- ContractNumber: {ContractNumber}", contract.ContractNumber);
            _logger.LogInformation("- CustomerId: {CustomerId}", contract.CustomerId);
            _logger.LogInformation("- Customer: {CustomerName}", contract.Customer?.FullName);

            // Gán thông tin hợp đồng trước khi validate
            invoice.Contract = contract;
            invoice.ContractId = contract.Id;

            // Tạo mã hóa đơn tự động
            var lastInvoice = await _invoiceRepository.GetLastInvoiceAsync();
            var invoiceNumber = 1;
            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Substring(2));
                invoiceNumber = lastNumber + 1;
            }
            invoice.InvoiceNumber = $"HD{invoiceNumber:D4}";

            _logger.LogInformation("Mã hóa đơn được tạo: {InvoiceNumber}", invoice.InvoiceNumber);

            // Đảm bảo Contract được load đầy đủ
            _logger.LogInformation("Kiểm tra thông tin Contract sau khi gán:");
            _logger.LogInformation("- Contract: {Contract}", invoice.Contract?.Id);
            _logger.LogInformation("- ContractId: {ContractId}", invoice.ContractId);
            _logger.LogInformation("- Contract.Customer: {Customer}", invoice.Contract?.Customer?.Id);
            _logger.LogInformation("- InvoiceNumber: {InvoiceNumber}", invoice.InvoiceNumber);

            // Clear ModelState để tránh validation lỗi
            ModelState.Clear();
            TryValidateModel(invoice);

            _logger.LogInformation("Thông tin hóa đơn sau khi gán hợp đồng:");
            _logger.LogInformation("- InvoiceId: {InvoiceId}", invoice.InvoiceId);
            _logger.LogInformation("- ContractId: {ContractId}", invoice.ContractId);
            _logger.LogInformation("- Contract: {Contract}", invoice.Contract?.Id);
            _logger.LogInformation("- Customer: {Customer}", invoice.Customer?.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    // Thêm hóa đơn vào database
                    var savedInvoice = await _invoiceRepository.AddAsync(invoice);
                    _logger.LogInformation("Đã thêm hóa đơn thành công với ID: {InvoiceId}", savedInvoice.InvoiceId);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo hóa đơn: {Message}", ex.Message);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo hóa đơn. Vui lòng thử lại.");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogWarning("Lỗi validation: {ErrorMessage}", error.ErrorMessage);
                }
            }

            // Nếu có lỗi, load lại dữ liệu cho dropdown
            var buildings = await _buildingRepository.GetAllBuildingsAsync();
            ViewBag.Buildings = new SelectList(buildings, "Id", "Name");

            // Nếu có ContractId, load dữ liệu cho dropdown phòng và hợp đồng
            if (invoice.ContractId > 0)
            {
                try
                {
                    // Lấy thông tin hợp đồng
                    var existingContract = await _contractRepository.GetByIdAsync(invoice.ContractId);
                    _logger.LogInformation("Load lại dữ liệu cho dropdown - Hợp đồng: {@Contract}", existingContract);
                    
                    if (existingContract != null && existingContract.Room != null)
                    {
                        var rooms = await _roomRepository.GetByBuildingIdAsync(existingContract.Room.BuildingId);
                        ViewBag.Rooms = new SelectList(rooms, "Id", "Name");

                        var contracts = await _contractRepository.GetContractsByRoomIdAsync(existingContract.RoomId);
                        ViewBag.Contracts = new SelectList(contracts, "Id", "ContractNumber");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi load dữ liệu cho dropdown: {Message}", ex.Message);
                }
            }

            return View(invoice);
        }

        // GET: InvoiceMvc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            // Load thông tin hợp đồng và phòng
            var contract = await _contractRepository.GetByIdAsync(invoice.ContractId);
            if (contract != null)
            {
                invoice.Contract = contract;
                if (contract.Room != null)
                {
                    ViewBag.BuildingId = contract.Room.BuildingId;
                    ViewBag.RoomId = contract.RoomId;
                }
            }

            return View(invoice);
        }

        // POST: InvoiceMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            _logger.LogInformation("Bắt đầu cập nhật hóa đơn {InvoiceId}", id);
            
            if (id != invoice.InvoiceId)
            {
                _logger.LogWarning("ID hóa đơn không khớp: {RequestId} != {InvoiceId}", id, invoice.InvoiceId);
                return NotFound();
            }

            // Kiểm tra hợp đồng tồn tại
            _logger.LogInformation("Đang tìm hợp đồng với ID: {ContractId}", invoice.ContractId);
            var contract = await _contractRepository.GetByIdAsync(invoice.ContractId);
            
            if (contract == null)
            {
                _logger.LogWarning("Không tìm thấy hợp đồng với ID: {ContractId}", invoice.ContractId);
                ModelState.AddModelError("", "Hợp đồng không tồn tại");
                return View(invoice);
            }

            _logger.LogInformation("Thông tin hợp đồng tìm được:");
            _logger.LogInformation("- ContractId: {ContractId}", contract.Id);
            _logger.LogInformation("- ContractNumber: {ContractNumber}", contract.ContractNumber);
            _logger.LogInformation("- CustomerId: {CustomerId}", contract.CustomerId);
            _logger.LogInformation("- Customer: {CustomerName}", contract.Customer?.FullName);

            // Lấy thông tin hóa đơn hiện tại
            var existingInvoice = await _invoiceRepository.GetByIdAsync(id);
            if (existingInvoice == null)
            {
                _logger.LogWarning("Không tìm thấy hóa đơn với ID: {InvoiceId}", id);
                return NotFound();
            }

            try
            {
                // Cập nhật các trường được phép sửa
                existingInvoice.DueDate = invoice.DueDate;
                existingInvoice.Status = invoice.Status;
                existingInvoice.PaidDate = invoice.PaidDate;
                existingInvoice.PaymentMethod = invoice.PaymentMethod;
                existingInvoice.ReferenceNumber = invoice.ReferenceNumber;
                existingInvoice.Notes = invoice.Notes;

                // Clear ModelState để tránh validation lỗi
                ModelState.Clear();

                // Chỉ validate các trường cần thiết
                if (string.IsNullOrEmpty(existingInvoice.InvoiceNumber))
                    ModelState.AddModelError("InvoiceNumber", "Mã hóa đơn là bắt buộc");
                if (existingInvoice.ContractId <= 0)
                    ModelState.AddModelError("ContractId", "Hợp đồng là bắt buộc");
                if (existingInvoice.DueDate == default)
                    ModelState.AddModelError("DueDate", "Hạn thanh toán là bắt buộc");
                if (string.IsNullOrEmpty(existingInvoice.Status.ToString()))
                    ModelState.AddModelError("Status", "Trạng thái là bắt buộc");

                if (ModelState.IsValid)
                {
                    // Cập nhật hóa đơn
                    await _invoiceRepository.UpdateAsync(existingInvoice);
                    _logger.LogInformation("Đã cập nhật hóa đơn thành công");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        _logger.LogWarning("Lỗi validation: {ErrorMessage}", error.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hóa đơn: {Message}", ex.Message);
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật hóa đơn. Vui lòng thử lại.");
            }

            // Nếu có lỗi, load lại dữ liệu cho view
            if (contract.Room != null)
            {
                ViewBag.BuildingId = contract.Room.BuildingId;
                ViewBag.RoomId = contract.RoomId;
            }

            return View(existingInvoice);
        }

        // GET: InvoiceMvc/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        // POST: InvoiceMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // API endpoint để lấy phòng theo tòa nhà
        [HttpGet]
        public async Task<IActionResult> GetRoomsByBuilding(int id)
        {
            var rooms = await _roomRepository.GetRoomsByBuildingIdAsync(id);
            return Json(rooms.Select(r => new { id = r.Id, name = r.Name }));
        }

        // API endpoint để lấy hợp đồng theo phòng
        [HttpGet]
        public async Task<IActionResult> GetContractsByRoom(int id)
        {
            var contracts = await _contractRepository.GetContractsByRoomIdAsync(id);
            return Json(contracts.Select(c => new { id = c.Id, contractNumber = c.ContractNumber }));
        }

        [HttpGet]
        public async Task<IActionResult> GetContractInfo(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            return Json(new 
            {
                paymentCycle = contract.PaymentCycle,
                // Có thể thêm các thông tin khác của hợp đồng nếu cần
            });
        }
    }
} 