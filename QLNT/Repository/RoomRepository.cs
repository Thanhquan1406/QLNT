using QLNT.Models;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QLNT.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(ApplicationDbContext context, ILogger<RoomRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<Room> AddAsync(Room room)
        {
            try
            {
                // Tạo mã phòng tự động
                var building = await _context.Buildings.FindAsync(room.BuildingId);
                if (building != null)
                {
                    var roomCount = await _context.Rooms.CountAsync(r => r.BuildingId == room.BuildingId);
                    room.Code = $"{building.Code}-{room.Floor}-{roomCount + 1:D3}";
                }

                room.Status = RoomStatus.Available;
                room.IsActive = true;

                // Nếu không có mẫu in hóa đơn, lấy từ tòa nhà
                if (string.IsNullOrEmpty(room.InvoiceTemplate))
                {
                    var buildingInfo = await _context.Buildings.FindAsync(room.BuildingId);
                    if (buildingInfo != null)
                    {
                        room.InvoiceTemplate = buildingInfo.InvoiceTemplate;
                    }
                }

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm phòng mới");
                throw;
            }
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetByBuildingIdAsync(int buildingId)
        {
            return await _context.Rooms
                .Where(r => r.BuildingId == buildingId)
                .ToListAsync();
        }
    }
}
