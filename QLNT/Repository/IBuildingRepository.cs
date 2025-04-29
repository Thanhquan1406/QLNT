using QLNT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IBuildingRepository
    {
        Task<IEnumerable<Building>> GetAllBuildingsAsync();
        Task<Building> GetBuildingByIdAsync(int id);
        Task<Building> CreateBuildingAsync(Building building);
        Task<Building> UpdateBuildingAsync(Building building);
        Task<bool> DeleteBuildingAsync(int id);
        Task<bool> BuildingExistsAsync(int id);
    }
} 