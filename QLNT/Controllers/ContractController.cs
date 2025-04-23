using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNT.Models;
using QLNT.Repository;

namespace QLNT.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractRepository _contractRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBuildingRepository _buildingRepository;

        public ContractController(
            IContractRepository contractRepository,
            IRoomRepository roomRepository,
            ICustomerRepository customerRepository,
            IBuildingRepository buildingRepository)
        {
            _contractRepository = contractRepository;
            _roomRepository = roomRepository;
            _customerRepository = customerRepository;
            _buildingRepository = buildingRepository;
        }

        // GET: Contract
        public async Task<IActionResult> Index()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return View(contracts);
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }

        // GET: Contract/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                await PopulateDropdowns();
                return View(model);
            }

            try
            {
                // Kiểm tra phòng có sẵn sàng không
                if (!await _contractRepository.IsRoomAvailableAsync(model.RoomId, model.StartDate, model.EndDate))
                {
                    ModelState.AddModelError("", "Phòng đã được thuê trong khoảng thời gian này");
                    await PopulateDropdowns();
                    return View(model);
                }

                // Kiểm tra khách hàng có tồn tại không
                var customer = await _customerRepository.GetByIdAsync(model.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError("", "Khách hàng không tồn tại");
                    await PopulateDropdowns();
                    return View(model);
                }

                // Kiểm tra phòng có tồn tại không
                var room = await _roomRepository.GetByIdAsync(model.RoomId);
                if (room == null)
                {
                    ModelState.AddModelError("", "Phòng không tồn tại");
                    await PopulateDropdowns();
                    return View(model);
                }

                var contract = new Contract
                {
                    ContractNumber = model.ContractNumber,
                    RoomId = model.RoomId,
                    CustomerId = model.CustomerId,
                    SignDate = model.SignDate,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Note = model.Note ?? string.Empty,
                    InvoiceTemplate = model.InvoiceTemplate ?? string.Empty,
                    Representative = model.Representative,
                    RentalPrice = model.RentalPrice,
                    PaymentCycle = model.PaymentCycle,
                    PaymentStartDate = model.PaymentStartDate,
                    Deposit = model.Deposit,
                    DepositPaid = model.DepositPaid,
                    DepositRemaining = model.DepositRemaining,
                    DiscountMonths = model.DiscountMonths,
                    MonthlyDiscount = model.MonthlyDiscount,
                    Status = ContractStatus.Active
                };

                // Log thông tin hợp đồng trước khi lưu
                Console.WriteLine($"Creating contract: {contract.ContractNumber}");
                Console.WriteLine($"RoomId: {contract.RoomId}");
                Console.WriteLine($"CustomerId: {contract.CustomerId}");
                Console.WriteLine($"StartDate: {contract.StartDate}");
                Console.WriteLine($"EndDate: {contract.EndDate}");

                await _contractRepository.AddAsync(contract);
                Console.WriteLine("Contract created successfully");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating contract: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo hợp đồng: " + ex.Message);
            }

            await PopulateDropdowns();
            return View(model);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            // Lấy thông tin phòng để lấy BuildingId
            var room = await _roomRepository.GetByIdAsync(contract.RoomId);
            if (room == null)
            {
                return NotFound();
            }

            var model = new ContractViewModel
            {
                Id = contract.Id,
                ContractNumber = contract.ContractNumber,
                BuildingId = room.BuildingId, // Thêm BuildingId từ phòng
                RoomId = contract.RoomId,
                CustomerId = contract.CustomerId,
                SignDate = contract.SignDate,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Note = contract.Note,
                InvoiceTemplate = contract.InvoiceTemplate,
                Representative = contract.Representative,
                RentalPrice = contract.RentalPrice,
                PaymentCycle = contract.PaymentCycle,
                PaymentStartDate = contract.PaymentStartDate,
                Deposit = contract.Deposit,
                DepositPaid = contract.DepositPaid,
                DepositRemaining = contract.DepositRemaining,
                DiscountMonths = contract.DiscountMonths,
                MonthlyDiscount = contract.MonthlyDiscount,
                Status = contract.Status
            };

            await PopulateDropdowns();
            return View(model);
        }

        // POST: Contract/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra phòng có sẵn sàng không
                if (!await _contractRepository.IsRoomAvailableAsync(model.RoomId, model.StartDate, model.EndDate, id))
                {
                    ModelState.AddModelError("", "Phòng đã được thuê trong khoảng thời gian này");
                    await PopulateDropdowns();
                    return View(model);
                }

                var contract = await _contractRepository.GetByIdAsync(id);
                if (contract == null)
                {
                    return NotFound();
                }

                contract.ContractNumber = model.ContractNumber;
                contract.RoomId = model.RoomId;
                contract.CustomerId = model.CustomerId;
                contract.SignDate = model.SignDate;
                contract.StartDate = model.StartDate;
                contract.EndDate = model.EndDate;
                contract.Note = model.Note;
                contract.InvoiceTemplate = model.InvoiceTemplate;
                contract.Representative = model.Representative;
                contract.RentalPrice = model.RentalPrice;
                contract.PaymentCycle = model.PaymentCycle;
                contract.PaymentStartDate = model.PaymentStartDate;
                contract.Deposit = model.Deposit;
                contract.DepositPaid = model.DepositPaid;
                contract.DepositRemaining = model.DepositRemaining;
                contract.DiscountMonths = model.DiscountMonths;
                contract.MonthlyDiscount = model.MonthlyDiscount;
                contract.Status = model.Status;

                await _contractRepository.UpdateAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns();
            return View(model);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contractRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contract/Active
        public async Task<IActionResult> Active()
        {
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return View(contracts);
        }

        // GET: Contract/Expiring
        public async Task<IActionResult> Expiring()
        {
            var contracts = await _contractRepository.GetContractsExpiringInDaysAsync(30);
            return View(contracts);
        }

        private async Task PopulateDropdowns()
        {
            var buildings = await _buildingRepository.GetAllAsync();
            var rooms = await _roomRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();

            if (buildings == null || rooms == null || customers == null)
            {
                throw new Exception("Không thể tải dữ liệu cho dropdown");
            }

            ViewBag.BuildingId = new SelectList(buildings, "Id", "Name");
            ViewBag.RoomId = new SelectList(rooms, "Id", "Name");
            ViewBag.CustomerId = new SelectList(customers, "Id", "FullName");
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByBuilding(int buildingId)
        {
            var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
            return Json(rooms.Select(r => new { id = r.Id, name = r.Name }));
        }
    }
} 