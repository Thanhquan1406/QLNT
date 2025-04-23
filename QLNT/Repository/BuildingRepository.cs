using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;

namespace QLNT.Repository
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly ApplicationDbContext _context;

        public BuildingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Building>> GetAllAsync()
        {
            return await _context.Buildings.ToListAsync();
        }

        public async Task<Building> GetBuildingByIdAsync(int id)
        {
            return await _context.Buildings.FindAsync(id);
        }

        public async Task<Building> AddBuildingAsync(Building building)
        {
            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task<Building> UpdateBuildingAsync(Building building)
        {
            _context.Entry(building).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task DeleteBuildingAsync(int id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building != null)
            {
                _context.Buildings.Remove(building);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BuildingExistsAsync(int id)
        {
            return await _context.Buildings.AnyAsync(b => b.Id == id);
        }
    }
} 