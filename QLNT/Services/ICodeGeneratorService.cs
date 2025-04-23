using System.Threading.Tasks;

namespace QLNT.Services
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateRoomCodeAsync(int buildingId);
        Task<string> GenerateBuildingCodeAsync();
    }
} 