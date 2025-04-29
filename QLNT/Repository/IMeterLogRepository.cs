using QLNT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IMeterLogRepository
    {
        Task<IEnumerable<MeterLog>> GetAllAsync();
        Task<MeterLog> GetByIdAsync(int id);
        Task<IEnumerable<MeterLog>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<MeterLog>> GetByRoomIdAndMonthAsync(int roomId, string month);
        Task<MeterLog> GetCurrentMeterAsync(int roomId, string meterType);
        Task<MeterLog> AddAsync(MeterLog meterLog);
        Task<MeterLog> UpdateAsync(MeterLog meterLog);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<double> CalculateConsumptionAsync(int roomId, string meterType, string month);
    }
}
