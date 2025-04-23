using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using QLNT.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QLNT.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNT.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomRepository roomRepository,
                            IBuildingRepository buildingRepository,
                            ICodeGeneratorService codeGeneratorService,
                            ApplicationDbContext context,
                            ILogger<RoomController> logger)
        {
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _codeGeneratorService = codeGeneratorService;
            _context = context;
            _logger = logger;
        }

        // GET: Room
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Building)
                .ToListAsync();

            return View(rooms);
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Room/Create
        public async Task<IActionResult> Create()
        {
            var buildings = await _context.Buildings
                .Where(b => b.IsActive)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            ViewBag.Buildings = new SelectList(buildings, "Value", "Text");
            return View();
        }

        // POST: Room/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildingId,Floor,Name,RentalFee,Deposit,Area,InvoiceTemplate")] Room room)
        {
            try
            {
                // Tự động sinh mã phòng
                room.Code = await _codeGeneratorService.GenerateRoomCodeAsync(room.BuildingId);
                _logger.LogInformation($"Generated room code: {room.Code}");

                // Thiết lập các giá trị mặc định
                room.Status = RoomStatus.Available;
                room.IsActive = true;

                // Lưu vào database
                await _roomRepository.AddAsync(room);
                _logger.LogInformation($"Room created successfully with ID: {room.Id}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Tạo phòng thành công" });
                }
                TempData["SuccessMessage"] = "Tạo phòng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating room: {ex.Message}");
                
                // Lấy lại danh sách tòa nhà
                var buildings = await _context.Buildings
                    .Where(b => b.IsActive)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    })
                    .ToListAsync();

                ViewBag.Buildings = new SelectList(buildings, "Value", "Text", room.BuildingId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, error = $"Lỗi khi tạo phòng: {ex.Message}" });
                }
                TempData["ErrorMessage"] = $"Lỗi khi tạo phòng: {ex.Message}";
                return View(room);
            }
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            // Lấy danh sách tòa nhà
            var buildings = await _context.Buildings
                .Where(b => b.IsActive)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            if (!buildings.Any())
            {
                TempData["ErrorMessage"] = "Không có tòa nhà nào đang hoạt động. Vui lòng tạo tòa nhà trước khi chỉnh sửa phòng.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Buildings = new SelectList(buildings, "Value", "Text", room.BuildingId);
            
            // Đảm bảo Code không null
            if (string.IsNullOrEmpty(room.Code))
            {
                room.Code = await _codeGeneratorService.GenerateRoomCodeAsync(room.BuildingId);
            }
            
            return View(room);
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,BuildingId,Floor,Name,RentalFee,Status,IsActive")] Room room)
        {
            Console.WriteLine("\n=== BẮT ĐẦU XỬ LÝ EDIT ROOM ===");
            Console.WriteLine($"Thời gian: {DateTime.Now}");
            Console.WriteLine($"Room ID: {id}");
            Console.WriteLine($"Dữ liệu nhận được từ form:");
            Console.WriteLine($"- BuildingId: {room.BuildingId}");
            Console.WriteLine($"- Floor: {room.Floor}");
            Console.WriteLine($"- Name: {room.Name}");
            Console.WriteLine($"- RentalFee: {room.RentalFee}");
            Console.WriteLine($"- Status: {room.Status}");
            Console.WriteLine($"- IsActive: {room.IsActive}");
            
            _logger.LogInformation($"Edit POST received for room {id} with BuildingId: {room.BuildingId}");
            
            if (id != room.Id)
            {
                Console.WriteLine("LỖI: ID không khớp");
                return NotFound();
            }

            try
            {
                Console.WriteLine("\n=== KIỂM TRA TÒA NHÀ ===");
                // Kiểm tra BuildingId có tồn tại và đang active không
                var building = await _context.Buildings
                    .FirstOrDefaultAsync(b => b.Id == room.BuildingId && b.IsActive);
                
                if (building == null)
                {
                    Console.WriteLine("LỖI: Tòa nhà không tồn tại hoặc không đang hoạt động");
                    ModelState.AddModelError("BuildingId", "Tòa nhà không tồn tại hoặc không đang hoạt động");
                    var buildings = await _context.Buildings
                        .Where(b => b.IsActive)
                        .Select(b => new SelectListItem
                        {
                            Value = b.Id.ToString(),
                            Text = b.Name
                        })
                        .ToListAsync();
                    ViewBag.Buildings = new SelectList(buildings, "Value", "Text", room.BuildingId);
                    return View(room);
                }
                Console.WriteLine($"Tòa nhà hợp lệ: {building.Name} (ID: {building.Id})");

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("\n=== LỖI VALIDATION ===");
                    Console.WriteLine("Chi tiết ModelState:");
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        Console.WriteLine($"Key: {key}");
                        Console.WriteLine($"RawValue: {state.RawValue}");
                        Console.WriteLine($"AttemptedValue: {state.AttemptedValue}");
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"Lỗi: {error.ErrorMessage}");
                        }
                    }
                    
                    // Lấy lại danh sách tòa nhà
                    var buildings = await _context.Buildings
                        .Where(b => b.IsActive)
                        .Select(b => new SelectListItem
                        {
                            Value = b.Id.ToString(),
                            Text = b.Name
                        })
                        .ToListAsync();

                    ViewBag.Buildings = new SelectList(buildings, "Value", "Text", room.BuildingId);
                    return View(room);
                }
                Console.WriteLine("Validation thành công");

                Console.WriteLine("\n=== LẤY DỮ LIỆU PHÒNG HIỆN TẠI ===");
                var existingRoom = await _roomRepository.GetByIdAsync(id);
                if (existingRoom == null)
                {
                    Console.WriteLine("LỖI: Không tìm thấy phòng");
                    return NotFound();
                }

                Console.WriteLine("Dữ liệu hiện tại của phòng:");
                Console.WriteLine($"- BuildingId: {existingRoom.BuildingId}");
                Console.WriteLine($"- Floor: {existingRoom.Floor}");
                Console.WriteLine($"- Name: {existingRoom.Name}");
                Console.WriteLine($"- RentalFee: {existingRoom.RentalFee}");
                Console.WriteLine($"- Status: {existingRoom.Status}");
                Console.WriteLine($"- IsActive: {existingRoom.IsActive}");

                // Kiểm tra BuildingId có thay đổi không
                if (existingRoom.BuildingId != room.BuildingId)
                {
                    Console.WriteLine("\n=== THAY ĐỔI TÒA NHÀ ===");
                    Console.WriteLine($"BuildingId thay đổi từ {existingRoom.BuildingId} -> {room.BuildingId}");
                    // Nếu thay đổi tòa nhà, sinh mã phòng mới
                    var newCode = await _codeGeneratorService.GenerateRoomCodeAsync(room.BuildingId);
                    existingRoom.Code = newCode;
                    Console.WriteLine($"Mã phòng mới: {newCode}");
                }

                Console.WriteLine("\n=== BẮT ĐẦU CẬP NHẬT DỮ LIỆU ===");
                Console.WriteLine("Dữ liệu cũ -> Dữ liệu mới");
                Console.WriteLine($"BuildingId: {existingRoom.BuildingId} -> {room.BuildingId}");
                Console.WriteLine($"Floor: {existingRoom.Floor} -> {room.Floor}");
                Console.WriteLine($"Name: {existingRoom.Name} -> {room.Name}");
                Console.WriteLine($"RentalFee: {existingRoom.RentalFee} -> {room.RentalFee}");
                Console.WriteLine($"Status: {existingRoom.Status} -> {room.Status}");
                Console.WriteLine($"IsActive: {existingRoom.IsActive} -> {room.IsActive}");

                Console.WriteLine("\n=== THỰC HIỆN GÁN DỮ LIỆU ===");
                existingRoom.BuildingId = room.BuildingId;
                Console.WriteLine($"Đã gán BuildingId: {existingRoom.BuildingId}");

                existingRoom.Floor = room.Floor;
                Console.WriteLine($"Đã gán Floor: {existingRoom.Floor}");

                existingRoom.Name = room.Name;
                Console.WriteLine($"Đã gán Name: {existingRoom.Name}");

                existingRoom.RentalFee = room.RentalFee;
                Console.WriteLine($"Đã gán RentalFee: {existingRoom.RentalFee}");

                existingRoom.Status = room.Status;
                Console.WriteLine($"Đã gán Status: {existingRoom.Status}");

                existingRoom.IsActive = room.IsActive;
                Console.WriteLine($"Đã gán IsActive: {existingRoom.IsActive}");

                Console.WriteLine("\n=== BẮT ĐẦU LƯU VÀO DATABASE ===");
                _context.Entry(existingRoom).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                Console.WriteLine("=== ĐÃ LƯU THÀNH CÔNG ===");

                _logger.LogInformation($"Room {id} updated successfully");
                TempData["SuccessMessage"] = "Cập nhật thông tin phòng thành công!";
                Console.WriteLine("\n=== KẾT THÚC XỬ LÝ EDIT ROOM ===");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n=== LỖI KHI CẬP NHẬT ===");
                Console.WriteLine($"Thời gian lỗi: {DateTime.Now}");
                Console.WriteLine($"Lỗi: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                _logger.LogError($"Error updating room: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật phòng: {ex.Message}";
                
                // Lấy lại danh sách tòa nhà
                var buildings = await _context.Buildings
                    .Where(b => b.IsActive)
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    })
                    .ToListAsync();

                ViewBag.Buildings = new SelectList(buildings, "Value", "Text", room.BuildingId);
                return View(room);
            }
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
