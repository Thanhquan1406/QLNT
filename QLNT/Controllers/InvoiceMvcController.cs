using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.Text.Json.Serialization;

namespace QLNT.Controllers
{
    public class InvoiceMvcController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMeterLogRepository _meterLogRepository;
        private readonly ILogger<InvoiceMvcController> _logger;
        private readonly ApplicationDbContext _context;

        public InvoiceMvcController(
            IInvoiceRepository invoiceRepository, 
            IContractRepository contractRepository,
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository,
            IServiceRepository serviceRepository,
            IMeterLogRepository meterLogRepository,
            ILogger<InvoiceMvcController> logger,
            ApplicationDbContext context)
        {
            _invoiceRepository = invoiceRepository;
            _contractRepository = contractRepository;
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
            _meterLogRepository = meterLogRepository;
            _logger = logger;
            _context = context;
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
            try
            {
                var buildings = await _buildingRepository.GetAllBuildingsAsync();
                var services = await _serviceRepository.GetAllServicesAsync();
                var lastInvoice = await _invoiceRepository.GetLastInvoiceAsync();

                ViewBag.Buildings = buildings;
                ViewBag.Services = services;
                ViewBag.LastInvoice = lastInvoice;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dữ liệu cho form tạo hóa đơn");
                ViewBag.Services = new List<Service>();
                return View();
            }
        }

        // POST: InvoiceMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice, string contractData, string invoiceDetails)
        {
            _logger.LogInformation($"Bắt đầu tạo hóa đơn mới");
            _logger.LogInformation($"ContractId: {invoice.ContractId}");
            _logger.LogInformation($"ContractData: {contractData}");
            _logger.LogInformation($"InvoiceDetails: {invoiceDetails}");

            try
            {
                // Parse InvoiceDetails từ JSON string
                if (!string.IsNullOrEmpty(invoiceDetails))
                {
                    var details = JsonSerializer.Deserialize<List<InvoiceDetail>>(invoiceDetails, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonDateTimeConverter() }
                    });
                    
                    if (details != null && details.Any())
                    {
                        // Xử lý thông tin MeterLog cho từng chi tiết
                        foreach (var detail in details)
                        {
                            if (detail.MeterLogId.HasValue)
                            {
                                var meterLog = await _meterLogRepository.GetByIdAsync(detail.MeterLogId.Value);
                                if (meterLog != null)
                                {
                                    detail.OldReading = (decimal?)meterLog.OldReading;
                                    detail.NewReading = (decimal?)meterLog.NewReading;
                                    detail.MeterName = meterLog.MeterName;
                                    detail.Month = meterLog.Month;
                                }
                            }

                            // Log thông tin ngày tháng
                            _logger.LogInformation($"Chi tiết hóa đơn - StartDate: {detail.StartDate}, EndDate: {detail.EndDate}");
                        }
                        invoice.InvoiceDetails = details;
                        _logger.LogInformation($"Đã parse được {details.Count} chi tiết hóa đơn");
                    }
                }

                if (ModelState.IsValid)
                {
                    // Lấy thông tin hợp đồng từ ContractId
                    var contract = await _contractRepository.GetByIdAsync(invoice.ContractId);
                    
                    if (contract == null)
                    {
                        _logger.LogWarning($"Không tìm thấy hợp đồng với ID: {invoice.ContractId}");
                        return Json(new { success = false, message = "Không tìm thấy hợp đồng" });
                    }

                    // Cập nhật thông tin từ hợp đồng
                    invoice.UpdateFromContract(contract);

                    // Tính toán tổng tiền
                    if (invoice.InvoiceDetails != null && invoice.InvoiceDetails.Any())
                    {
                        _logger.LogInformation($"Số lượng chi tiết hóa đơn: {invoice.InvoiceDetails.Count}");
                        foreach (var detail in invoice.InvoiceDetails)
                        {
                            _logger.LogInformation($"Chi tiết: ServiceId={detail.ServiceId}, Quantity={detail.Quantity}, Amount={detail.Amount}");
                        }
                        invoice.ServiceAmount = invoice.InvoiceDetails.Sum(d => d.Amount);
                        invoice.TotalAmount = (invoice.RentAmount ?? 0) + (invoice.ServiceAmount ?? 0) - invoice.Discount;
                    }
                    else
                    {
                        _logger.LogWarning("Không có chi tiết hóa đơn nào");
                        return Json(new { success = false, message = "Vui lòng thêm ít nhất một dịch vụ" });
                    }

                    // Lưu hóa đơn
                    await _invoiceRepository.AddAsync(invoice);
                    _logger.LogInformation($"Tạo hóa đơn thành công. ID: {invoice.InvoiceId}");
                    return Json(new { success = true, redirectUrl = Url.Action("Index") });
                }
                else
                {
                    _logger.LogWarning("Dữ liệu không hợp lệ");
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        // GET: InvoiceMvc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                // Lấy danh sách tòa nhà và dịch vụ
                var buildings = await _buildingRepository.GetAllBuildingsAsync();
                var services = await _serviceRepository.GetAllServicesAsync();

                ViewBag.Buildings = buildings;
                ViewBag.Services = services;

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dữ liệu cho form chỉnh sửa hóa đơn");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: InvoiceMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice, string contractData, string invoiceDetails)
        {
            _logger.LogInformation($"Bắt đầu cập nhật hóa đơn {id}");
            
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            try
            {
                // Parse InvoiceDetails từ JSON string
                if (!string.IsNullOrEmpty(invoiceDetails))
                {
                    var details = JsonSerializer.Deserialize<List<InvoiceDetail>>(invoiceDetails);
                    if (details != null && details.Any())
                    {
                        invoice.InvoiceDetails = details;
                        _logger.LogInformation($"Đã parse được {details.Count} chi tiết hóa đơn");
                    }
                }

                if (ModelState.IsValid)
                {
                    // Lấy thông tin hợp đồng từ ContractId
                    var contract = await _contractRepository.GetByIdAsync(invoice.ContractId);
                    
                    if (contract == null)
                    {
                        _logger.LogWarning($"Không tìm thấy hợp đồng với ID: {invoice.ContractId}");
                        return Json(new { success = false, message = "Không tìm thấy hợp đồng" });
                    }

                    // Cập nhật thông tin từ hợp đồng
                    invoice.UpdateFromContract(contract);

                    // Tính toán tổng tiền
                    if (invoice.InvoiceDetails != null && invoice.InvoiceDetails.Any())
                    {
                        _logger.LogInformation($"Số lượng chi tiết hóa đơn: {invoice.InvoiceDetails.Count}");
                        foreach (var detail in invoice.InvoiceDetails)
                        {
                            _logger.LogInformation($"Chi tiết: ServiceId={detail.ServiceId}, Quantity={detail.Quantity}, Amount={detail.Amount}");
                        }
                        invoice.ServiceAmount = invoice.InvoiceDetails.Sum(d => d.Amount);
                        invoice.TotalAmount = (invoice.RentAmount ?? 0) + (invoice.ServiceAmount ?? 0) - invoice.Discount;
                    }
                    else
                    {
                        _logger.LogWarning("Không có chi tiết hóa đơn nào");
                        return Json(new { success = false, message = "Vui lòng thêm ít nhất một dịch vụ" });
                    }

                    // Cập nhật hóa đơn
                    await _invoiceRepository.UpdateAsync(invoice);
                    _logger.LogInformation($"Cập nhật hóa đơn thành công. ID: {invoice.InvoiceId}");
                    return Json(new { success = true, redirectUrl = Url.Action("Index") });
                }
                else
                {
                    _logger.LogWarning("Dữ liệu không hợp lệ");
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hóa đơn");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
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

        // GET: InvoiceMvc/GetRoomsByBuilding/5
        public async Task<IActionResult> GetRoomsByBuilding(int buildingId)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu lấy danh sách phòng cho tòa nhà {buildingId}");

                if (buildingId <= 0)
                {
                    return Json(new List<object>());
                }

                var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
                var roomList = rooms.Select(r => new
                {
                    id = r.Id,
                    name = $"{r.Code} - {r.Name}"
                }).ToList();

                return Json(roomList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy danh sách phòng cho tòa nhà {buildingId}");
                return Json(new List<object>());
            }
        }

        // GET: InvoiceMvc/GetContractsByRoom/5
        public async Task<IActionResult> GetContractsByRoom(int roomId)
        {
            try
            {
                _logger.LogInformation($"=== BẮT ĐẦU LẤY HỢP ĐỒNG CHO PHÒNG {roomId} ===");

                if (roomId <= 0)
                {
                    _logger.LogWarning($"RoomId không hợp lệ: {roomId}");
                    return Json(new List<object>());
                }

                var contracts = await _contractRepository.GetByRoomIdAsync(roomId);
                _logger.LogInformation($"Số lượng hợp đồng tìm thấy: {contracts.Count()}");
                
                var contractList = contracts.Select(c => {
                    _logger.LogInformation($"=== THÔNG TIN HỢP ĐỒNG ===");
                    _logger.LogInformation($"ID: {c.Id} (Kiểu: {c.Id.GetType()})");
                    _logger.LogInformation($"Số hợp đồng: {c.ContractNumber}");
                    _logger.LogInformation($"Khách hàng: {c.Customer?.FullName}");
                    _logger.LogInformation($"Giá trị trả về cho dropdown: {new { id = c.Id, contractNumber = c.ContractNumber, customerName = c.Customer?.FullName }}");
                    return new
                    {
                        id = c.Id,
                        contractNumber = c.ContractNumber,
                        customerName = c.Customer?.FullName
                    };
                }).ToList();

                _logger.LogInformation($"=== DANH SÁCH TRẢ VỀ CHO CLIENT ===");
                _logger.LogInformation(JsonSerializer.Serialize(contractList));
                _logger.LogInformation("=== KẾT THÚC LẤY DANH SÁCH HỢP ĐỒNG ===\n");
                return Json(contractList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy danh sách hợp đồng cho phòng {roomId}");
                return Json(new List<object>());
            }
        }

        // GET: InvoiceMvc/GetContractInfo/5
        [HttpGet]
        [Route("InvoiceMvc/GetContractInfo/{contractId}")]
        public async Task<IActionResult> GetContractInfo(int contractId)
        {
            _logger.LogInformation($"\n=== BẮT ĐẦU LẤY THÔNG TIN HỢP ĐỒNG ===");
            _logger.LogInformation($"URL gọi đến: {Request.Path}");
            _logger.LogInformation($"Tham số contractId nhận được: {contractId} (Kiểu: {contractId.GetType()})");
            
            if (contractId <= 0)
            {
                _logger.LogWarning($"ContractId không hợp lệ: {contractId}");
                return NotFound();
            }

            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                _logger.LogWarning($"Không tìm thấy hợp đồng với ID: {contractId}");
                return NotFound();
            }

            _logger.LogInformation($"=== THÔNG TIN HỢP ĐỒNG TÌM THẤY ===");
            _logger.LogInformation($"ID: {contract.Id} (Kiểu: {contract.Id.GetType()})");
            _logger.LogInformation($"Số hợp đồng: {contract.ContractNumber}");
            _logger.LogInformation($"Khách hàng: {contract.Customer?.FullName}");
            _logger.LogInformation($"Giá thuê: {contract.RentalPrice}");
            _logger.LogInformation($"Kỳ thanh toán: {contract.PaymentCycle}");
            _logger.LogInformation("=== KẾT THÚC LẤY THÔNG TIN HỢP ĐỒNG ===\n");
            
            // Chỉ trả về các thông tin cần thiết
            var result = new
            {
                id = contract.Id,
                contractNumber = contract.ContractNumber,
                customerName = contract.Customer?.FullName,
                rentalPrice = contract.RentalPrice,
                paymentCycle = contract.PaymentCycle,
                startDate = contract.StartDate,
                endDate = contract.EndDate,
                status = contract.Status
            };
            
            return Json(result);
        }

        // GET: InvoiceMvc/GetMeterReadings
        public async Task<IActionResult> GetMeterReadings(int roomId, string serviceType)
        {
            var meterLogs = await _meterLogRepository.GetByRoomIdAsync(roomId);
            var filteredLogs = meterLogs.Where(m => m.MeterType == serviceType)
                                      .OrderByDescending(m => m.ReadingDate)
                                      .Select(m => new {
                                          id = m.Id,
                                          oldReading = m.OldReading,
                                          newReading = m.NewReading,
                                          consumption = m.Consumption,
                                          readingDate = m.ReadingDate.ToString("yyyy-MM-dd"),
                                          displayText = $"{m.MeterName} - {m.Month}: {m.OldReading} -> {m.NewReading} ({m.Consumption})"
                                      })
                                      .ToList();
            return Json(filteredLogs);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }
                
                invoice.IsApproved = true;
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class JsonDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            else
                writer.WriteNullValue();
        }
    }
} 