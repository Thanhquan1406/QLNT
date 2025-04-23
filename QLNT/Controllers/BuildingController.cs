using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using QLNT.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QLNT.Data;
using System;
using System.Collections.Generic;

namespace QLNT.Controllers
{
    public class BuildingController : Controller
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly ApplicationDbContext _context;

        public BuildingController(IBuildingRepository buildingRepository, 
                                ICodeGeneratorService codeGeneratorService,
                                ApplicationDbContext context)
        {
            _buildingRepository = buildingRepository;
            _codeGeneratorService = codeGeneratorService;
            _context = context;
        }

        // GET: Building
        public async Task<IActionResult> Index()
        {
            var buildings = await _context.Buildings
                .Include(b => b.Rooms)
                .Select(b => new BuildingViewModel
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name,
                    ShortName = b.ShortName,
                    RoomCount = b.Rooms.Count,
                    Address = b.Address,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return View(buildings);
        }

        // GET: Building/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var building = await _buildingRepository.GetBuildingByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return View(building);
        }

        // GET: Building/Create
        public async Task<IActionResult> Create()
        {
            var building = new Building
            {
                Code = await _codeGeneratorService.GenerateBuildingCodeAsync()
            };
            return View(building);
        }

        // POST: Building/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,ShortName,Address,Ward,District,Province,IsActive,InvoiceTemplate")] Building building)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo mã tự động
                    var generatedCode = await _codeGeneratorService.GenerateBuildingCodeAsync();
                    if (string.IsNullOrEmpty(generatedCode))
                    {
                        ModelState.AddModelError("", "Không thể tạo mã tự động. Vui lòng thử lại.");
                        return View(building);
                    }
                    
                    building.Code = generatedCode;
                    building.Rooms = new List<Room>();
                    
                    _context.Buildings.Add(building);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }
            return View(building);
        }

        // GET: Building/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var building = await _buildingRepository.GetBuildingByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return View(building);
        }

        // POST: Building/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,ShortName,Address,Ward,District,Province,IsActive,InvoiceTemplate")] Building building)
        {
            if (id != building.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBuilding = await _context.Buildings.FindAsync(id);
                    if (existingBuilding == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính
                    existingBuilding.Name = building.Name;
                    existingBuilding.ShortName = building.ShortName;
                    existingBuilding.Address = building.Address;
                    existingBuilding.Ward = building.Ward;
                    existingBuilding.District = building.District;
                    existingBuilding.Province = building.Province;
                    existingBuilding.IsActive = building.IsActive;
                    existingBuilding.InvoiceTemplate = building.InvoiceTemplate;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _buildingRepository.BuildingExistsAsync(building.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(building);
        }

        // GET: Building/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var building = await _buildingRepository.GetBuildingByIdAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return View(building);
        }

        // POST: Building/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _buildingRepository.DeleteBuildingAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

    public class BuildingViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int RoomCount { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
