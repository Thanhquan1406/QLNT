using QLNT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IBuildingRepository
    {
        Task<IEnumerable<Building>> GetAllAsync();
        Task<Building> GetBuildingByIdAsync(int id);
        Task<Building> AddBuildingAsync(Building building);
        Task<Building> UpdateBuildingAsync(Building building);
        Task DeleteBuildingAsync(int id);
        Task<bool> BuildingExistsAsync(int id);
    }
} 