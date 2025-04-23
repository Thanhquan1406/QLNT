using System.Collections.Generic;
using System.Threading.Tasks;
using QLNT.Models;

namespace QLNT.Repository
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(int id);
        Task<Room> AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Room>> GetByBuildingIdAsync(int buildingId);
    }
}
