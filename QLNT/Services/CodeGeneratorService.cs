using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;
using Microsoft.Extensions.Logging;

namespace QLNT.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CodeGeneratorService> _logger;

        public CodeGeneratorService(ApplicationDbContext context, ILogger<CodeGeneratorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateBuildingCodeAsync()
        {
            try
            {
                // Lấy số lượng tòa nhà hiện có
                var buildingCount = await _context.Buildings.CountAsync();
                
                // Tạo mã mới dựa trên số thứ tự
                var newCode = $"B{(buildingCount + 1):D4}";
                
                return newCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mã tòa nhà");
                throw;
            }
        }

        public async Task<string> GenerateRoomCodeAsync(int buildingId)
        {
            try
            {
                // Lấy số lượng phòng hiện có trong tòa nhà
                var roomCount = await _context.Rooms
                    .Where(r => r.BuildingId == buildingId)
                    .CountAsync();
                
                // Lấy mã tòa nhà
                var building = await _context.Buildings.FindAsync(buildingId);
                if (building == null)
                {
                    _logger.LogError("Không tìm thấy tòa nhà với ID: {BuildingId}", buildingId);
                    throw new ArgumentException($"Không tìm thấy tòa nhà với ID: {buildingId}");
                }
                
                // Tạo mã mới dựa trên mã tòa nhà và số thứ tự phòng
                var newCode = $"{building.Code}-P{(roomCount + 1):D3}";
                
                return newCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mã phòng cho tòa nhà {BuildingId}", buildingId);
                throw;
            }
        }
    }
} 