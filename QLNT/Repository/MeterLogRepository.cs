using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public class MeterLogRepository : IMeterLogRepository
    {
        private readonly ApplicationDbContext _context;

        public MeterLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MeterLog>> GetAllAsync()
        {
            return await _context.MeterLogs
                .Include(ml => ml.Room)
                .ThenInclude(r => r.Building)
                .OrderByDescending(ml => ml.ReadingDate)
                .ToListAsync();
        }

        public async Task<MeterLog> GetByIdAsync(int id)
        {
            return await _context.MeterLogs
                .Include(ml => ml.Room)
                .ThenInclude(r => r.Building)
                .FirstOrDefaultAsync(ml => ml.Id == id);
        }

        public async Task<IEnumerable<MeterLog>> GetByRoomIdAsync(int roomId)
        {
            return await _context.MeterLogs
                .Include(ml => ml.Room)
                .ThenInclude(r => r.Building)
                .Where(ml => ml.RoomId == roomId)
                .OrderByDescending(ml => ml.ReadingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MeterLog>> GetByRoomIdAndMonthAsync(int roomId, string month)
        {
            return await _context.MeterLogs
                .Include(ml => ml.Room)
                .ThenInclude(r => r.Building)
                .Where(ml => ml.RoomId == roomId && ml.Month == month)
                .OrderByDescending(ml => ml.ReadingDate)
                .ToListAsync();
        }

        public async Task<MeterLog> GetCurrentMeterAsync(int roomId, string meterType)
        {
            return await _context.MeterLogs
                .Include(ml => ml.Room)
                .ThenInclude(r => r.Building)
                .Where(ml => ml.RoomId == roomId && 
                            ml.MeterType == meterType && 
                            ml.IsCurrentMeter)
                .FirstOrDefaultAsync();
        }

        public async Task<MeterLog> AddAsync(MeterLog meterLog)
        {
            // Cập nhật trạng thái IsCurrentMeter của các bản ghi cũ
            var oldCurrentMeter = await GetCurrentMeterAsync(meterLog.RoomId, meterLog.MeterType);
            if (oldCurrentMeter != null)
            {
                oldCurrentMeter.IsCurrentMeter = false;
                _context.MeterLogs.Update(oldCurrentMeter);
            }

            // Thêm bản ghi mới
            meterLog.IsCurrentMeter = true;
            meterLog.CreatedAt = DateTime.Now;
            await _context.MeterLogs.AddAsync(meterLog);
            await _context.SaveChangesAsync();
            return meterLog;
        }

        public async Task<MeterLog> UpdateAsync(MeterLog meterLog)
        {
            meterLog.UpdatedAt = DateTime.Now;
            _context.MeterLogs.Update(meterLog);
            await _context.SaveChangesAsync();
            return meterLog;
        }

        public async Task DeleteAsync(int id)
        {
            var meterLog = await GetByIdAsync(id);
            if (meterLog != null)
            {
                _context.MeterLogs.Remove(meterLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MeterLogs.AnyAsync(ml => ml.Id == id);
        }

        public async Task<double> CalculateConsumptionAsync(int roomId, string meterType, string month)
        {
            var meterLogs = await GetByRoomIdAndMonthAsync(roomId, month);
            var relevantLogs = meterLogs.Where(ml => ml.MeterType == meterType).ToList();

            if (relevantLogs.Count < 2)
                return 0;

            var firstLog = relevantLogs.OrderBy(ml => ml.ReadingDate).First();
            var lastLog = relevantLogs.OrderByDescending(ml => ml.ReadingDate).First();

            return lastLog.NewReading - firstLog.OldReading;
        }
    }
}
