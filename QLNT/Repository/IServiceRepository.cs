using QLNT.Models;

namespace QLNT.Repository
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service> GetServiceByIdAsync(int id);
        Task<Service> CreateServiceAsync(Service service);
        Task<Service> UpdateServiceAsync(Service service);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<Service>> GetServicesByBuildingIdAsync(int buildingId);
        Task<IEnumerable<Service>> GetServicesByTypeAsync(ServiceTypes serviceType);
        Task<bool> AddServiceToBuildingAsync(int serviceId, int buildingId);
        Task<bool> RemoveServiceFromBuildingAsync(int serviceId, int buildingId);
        Task<string> GenerateServiceCodeAsync(ServiceTypes serviceType);
    }
} 