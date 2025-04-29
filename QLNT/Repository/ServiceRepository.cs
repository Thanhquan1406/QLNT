using Microsoft.EntityFrameworkCore;
using QLNT.Models;
using QLNT.Data;
using Microsoft.Extensions.Logging;

namespace QLNT.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(ApplicationDbContext context, ILogger<ServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            _logger.LogInformation("Lấy danh sách tất cả dịch vụ");
            return await _context.Services
                .Include(s => s.BuildingServices)
                .ThenInclude(bs => bs.Building)
                .ToListAsync();
        }

        public async Task<Service> GetServiceByIdAsync(int id)
        {
            _logger.LogInformation($"Lấy thông tin dịch vụ với ID: {id}");
            return await _context.Services
                .Include(s => s.BuildingServices)
                .ThenInclude(bs => bs.Building)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<string> GenerateServiceCodeAsync(ServiceTypes serviceType)
        {
            try
            {
                _logger.LogInformation($"Tạo mã dịch vụ cho loại: {serviceType}");
                
                // Lấy mã dịch vụ lớn nhất hiện có
                var maxServiceCode = await _context.Services
                    .Where(s => s.ServiceCode.StartsWith("DV"))
                    .Select(s => s.ServiceCode)
                    .OrderByDescending(code => code)
                    .FirstOrDefaultAsync();

                int newId = 1;
                if (!string.IsNullOrEmpty(maxServiceCode))
                {
                    // Lấy số từ mã dịch vụ hiện có và tăng lên 1
                    if (int.TryParse(maxServiceCode.Substring(2), out int currentId))
                    {
                        newId = currentId + 1;
                    }
                }

                string serviceCode = $"DV{newId:D5}";
                _logger.LogInformation($"Mã dịch vụ được tạo: {serviceCode}");
                return serviceCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mã dịch vụ");
                return "DV00001";
            }
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu tạo dịch vụ mới: {service.ServiceName}");
                _logger.LogInformation($"Mã dịch vụ hiện tại: {(string.IsNullOrEmpty(service.ServiceCode) ? "Chưa có" : service.ServiceCode)}");
                
                // Kiểm tra xem mã dịch vụ đã tồn tại chưa
                if (!string.IsNullOrEmpty(service.ServiceCode))
                {
                    var existingService = await _context.Services
                        .FirstOrDefaultAsync(s => s.ServiceCode == service.ServiceCode);
                    
                    if (existingService != null)
                    {
                        _logger.LogWarning($"Mã dịch vụ {service.ServiceCode} đã tồn tại");
                        throw new Exception($"Mã dịch vụ {service.ServiceCode} đã tồn tại");
                    }
                    _logger.LogInformation($"Sử dụng mã dịch vụ đã nhập: {service.ServiceCode}");
                }
                else
                {
                    // Tạo mã dịch vụ mới nếu chưa có
                    service.ServiceCode = await GenerateServiceCodeAsync(service.ServiceType);
                    _logger.LogInformation($"Đã tạo mã dịch vụ mới: {service.ServiceCode}");
                }
                
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Tạo dịch vụ thành công. ID: {service.Id}, Mã: {service.ServiceCode}");
                return service;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo dịch vụ: {service.ServiceName}");
                throw;
            }
        }

        public async Task<Service> UpdateServiceAsync(Service service)
        {
            try
            {
                _logger.LogInformation($"Cập nhật dịch vụ ID: {service.Id}");
                _context.Entry(service).State = EntityState.Modified;
                _context.Entry(service).Property(x => x.ServiceCode).IsModified = false;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cập nhật dịch vụ thành công. ID: {service.Id}");
                return service;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật dịch vụ ID: {service.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Xóa dịch vụ ID: {id}");
                var service = await _context.Services
                    .Include(s => s.BuildingServices)
                    .FirstOrDefaultAsync(s => s.Id == id);
                
                if (service == null)
                {
                    _logger.LogWarning($"Không tìm thấy dịch vụ để xóa. ID: {id}");
                    return false;
                }

                // Xóa tất cả các bản ghi BuildingServices liên quan
                _context.BuildingServices.RemoveRange(service.BuildingServices);
                
                // Xóa dịch vụ
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Xóa dịch vụ thành công. ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa dịch vụ ID: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Service>> GetServicesByBuildingIdAsync(int buildingId)
        {
            _logger.LogInformation($"Lấy danh sách dịch vụ theo tòa nhà ID: {buildingId}");
            return await _context.Services
                .Include(s => s.BuildingServices)
                .Where(s => s.BuildingServices.Any(bs => bs.BuildingId == buildingId && bs.IsActive))
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByTypeAsync(ServiceTypes serviceType)
        {
            _logger.LogInformation($"Lấy danh sách dịch vụ theo loại: {serviceType}");
            return await _context.Services
                .Include(s => s.BuildingServices)
                .Where(s => s.ServiceType == serviceType)
                .ToListAsync();
        }

        public async Task<bool> AddServiceToBuildingAsync(int serviceId, int buildingId)
        {
            try
            {
                _logger.LogInformation($"Thêm dịch vụ {serviceId} vào tòa nhà {buildingId}");
                var buildingService = new BuildingService
                {
                    BuildingId = buildingId,
                    ServiceId = serviceId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.BuildingServices.Add(buildingService);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Thêm dịch vụ vào tòa nhà thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi thêm dịch vụ {serviceId} vào tòa nhà {buildingId}");
                throw;
            }
        }

        public async Task<bool> RemoveServiceFromBuildingAsync(int serviceId, int buildingId)
        {
            try
            {
                _logger.LogInformation($"Xóa dịch vụ {serviceId} khỏi tòa nhà {buildingId}");
                var buildingService = await _context.BuildingServices
                    .FirstOrDefaultAsync(bs => bs.ServiceId == serviceId && bs.BuildingId == buildingId);

                if (buildingService == null)
                {
                    _logger.LogWarning($"Không tìm thấy liên kết dịch vụ-tòa nhà để xóa");
                    return false;
                }

                buildingService.IsActive = false;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Xóa dịch vụ khỏi tòa nhà thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa dịch vụ {serviceId} khỏi tòa nhà {buildingId}");
                throw;
            }
        }
    }
} 