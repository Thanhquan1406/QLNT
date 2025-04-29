using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using QLNT.Models.ViewModels;

namespace QLNT.Controllers
{
    public class MeterLogController : Controller
    {
        private readonly IMeterLogRepository _meterLogRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;

        public MeterLogController(IMeterLogRepository meterLogRepository, IRoomRepository roomRepository, IBuildingRepository buildingRepository)
        {
            _meterLogRepository = meterLogRepository;
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
        }

        // GET: MeterLog
        public async Task<IActionResult> Index()
        {
            var meterLogs = await _meterLogRepository.GetAllAsync();
            return View(meterLogs);
        }

        // GET: MeterLog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var meterLog = await _meterLogRepository.GetByIdAsync(id);
            if (meterLog == null)
            {
                return NotFound();
            }
            return View(meterLog);
        }

        // GET: MeterLog/Create
        public async Task<IActionResult> Create()
        {
            var buildings = await _buildingRepository.GetAllBuildingsAsync();
            ViewBag.Buildings = buildings?.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }) ?? new List<SelectListItem>();

            var rooms = await _roomRepository.GetAllAsync();
            ViewBag.Rooms = rooms?.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }) ?? new List<SelectListItem>();

            return View();
        }

        // POST: MeterLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterLogCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Chi tiết lỗi: " + string.Join(", ", errors);
                    
                    // Load lại dữ liệu cho dropdown
                    var rooms = await _roomRepository.GetAllAsync();
                    ViewBag.Rooms = rooms?.Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name
                    }) ?? new List<SelectListItem>();
                    
                    return View(model);
                }

                // Kiểm tra phòng có tồn tại không
                var room = await _roomRepository.GetByIdAsync(model.RoomId);
                if (room == null)
                {
                    TempData["ErrorMessage"] = "Phòng không tồn tại trong hệ thống";
                    return View(model);
                }

                // Kiểm tra chỉ số mới phải lớn hơn chỉ số cũ
                if (model.NewReading <= model.OldReading)
                {
                    TempData["ErrorMessage"] = "Chỉ số mới phải lớn hơn chỉ số cũ";
                    return View(model);
                }

                var meterLog = new MeterLog
                {
                    RoomId = model.RoomId,
                    MeterType = model.MeterType,
                    MeterName = model.MeterName,
                    OldReading = model.OldReading,
                    NewReading = model.NewReading,
                    Month = model.Month,
                    ReadingDate = model.ReadingDate
                };

                await _meterLogRepository.AddAsync(meterLog);
                TempData["SuccessMessage"] = $"Đã thêm thành công ghi chỉ số {model.MeterType} cho phòng {room.Name}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi thêm ghi chỉ số: {ex.Message}";
                return View(model);
            }
        }

        // GET: MeterLog/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var meterLog = await _meterLogRepository.GetByIdAsync(id);
            if (meterLog == null)
            {
                return NotFound();
            }

            var model = new MeterLogCreateViewModel
            {
                Id = meterLog.Id,
                RoomId = meterLog.RoomId,
                MeterType = meterLog.MeterType,
                MeterName = meterLog.MeterName,
                OldReading = meterLog.OldReading,
                NewReading = meterLog.NewReading,
                Month = meterLog.Month,
                ReadingDate = meterLog.ReadingDate
            };

            ViewBag.Rooms = await _roomRepository.GetAllAsync();
            return View(model);
        }

        // POST: MeterLog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeterLogCreateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var meterLog = await _meterLogRepository.GetByIdAsync(id);
                if (meterLog == null)
                {
                    return NotFound();
                }

                meterLog.RoomId = model.RoomId;
                meterLog.MeterType = model.MeterType;
                meterLog.MeterName = model.MeterName;
                meterLog.OldReading = model.OldReading;
                meterLog.NewReading = model.NewReading;
                meterLog.Month = model.Month;
                meterLog.ReadingDate = model.ReadingDate;

                await _meterLogRepository.UpdateAsync(meterLog);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Rooms = await _roomRepository.GetAllAsync();
            return View(model);
        }

        // GET: MeterLog/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var meterLog = await _meterLogRepository.GetByIdAsync(id);
            if (meterLog == null)
            {
                return NotFound();
            }
            return View(meterLog);
        }

        // POST: MeterLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _meterLogRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: MeterLog/ByRoom/5
        public async Task<IActionResult> ByRoom(int roomId)
        {
            var meterLogs = await _meterLogRepository.GetByRoomIdAsync(roomId);
            return View("Index", meterLogs);
        }

        // GET: MeterLog/ByRoomAndMonth/5?month=2024-01
        public async Task<IActionResult> ByRoomAndMonth(int roomId, string month)
        {
            var meterLogs = await _meterLogRepository.GetByRoomIdAndMonthAsync(roomId, month);
            return View("Index", meterLogs);
        }

        // GET: MeterLog/CurrentMeter/5?meterType=Electric
        public async Task<IActionResult> CurrentMeter(int roomId, string meterType)
        {
            var meterLog = await _meterLogRepository.GetCurrentMeterAsync(roomId, meterType);
            if (meterLog == null)
            {
                return NotFound();
            }
            return View("Details", meterLog);
        }

        // GET: MeterLog/CalculateConsumption/5?meterType=Electric&month=2024-01
        public async Task<IActionResult> CalculateConsumption(int roomId, string meterType, string month)
        {
            var consumption = await _meterLogRepository.CalculateConsumptionAsync(roomId, meterType, month);
            return Json(new { consumption });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByBuilding(int buildingId)
        {
            var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
            var result = rooms.Select(r => new { id = r.Id, name = r.Name }).ToList();
            return Json(result);
        }

        // GET: MeterLog/ToggleApproval/5
        public async Task<IActionResult> ToggleApproval(int id)
        {
            var meterLog = await _meterLogRepository.GetByIdAsync(id);
            if (meterLog == null)
            {
                return NotFound();
            }

            // Đảo ngược trạng thái phê duyệt
            meterLog.IsCurrentMeter = !meterLog.IsCurrentMeter;
            await _meterLogRepository.UpdateAsync(meterLog);

            TempData["SuccessMessage"] = meterLog.IsCurrentMeter 
                ? "Đã phê duyệt ghi chỉ số thành công" 
                : "Đã hủy phê duyệt ghi chỉ số thành công";

            return RedirectToAction(nameof(Index));
        }
    }
}
