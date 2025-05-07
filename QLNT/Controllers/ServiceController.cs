using Microsoft.AspNetCore.Mvc;
using QLNT.Models;
using QLNT.Repository;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace QLNT.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(IServiceRepository serviceRepository, 
                               IBuildingRepository buildingRepository,
                               ILogger<ServiceController> logger)
        {
            _serviceRepository = serviceRepository;
            _buildingRepository = buildingRepository;
            _logger = logger;
        }

        // GET: Service
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Truy cập trang danh sách dịch vụ");
            var services = await _serviceRepository.GetAllServicesAsync();
            return View(services);
        }

        // GET: Service/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Xem chi tiết dịch vụ ID: {id}");
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning($"Không tìm thấy dịch vụ ID: {id}");
                return NotFound();
            }
            return View(service);
        }

        // GET: Service/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Truy cập trang tạo dịch vụ mới");
            var buildings = await _buildingRepository.GetAllBuildingsAsync();
            var viewModel = new ServiceViewModel
            {
                Buildings = buildings.ToList(),
                ServiceType = ServiceTypes.RentFee,
                PriceType = PriceTypes.FixedPerMonth,
                Unit = UnitTypes.Room
            };
            return View(viewModel);
        }

        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel viewModel)
        {
            _logger.LogInformation($"Bắt đầu tạo dịch vụ mới: {viewModel.ServiceName}");
            _logger.LogInformation($"Mã dịch vụ từ form: {(string.IsNullOrEmpty(viewModel.ServiceCode) ? "Chưa nhập" : viewModel.ServiceCode)}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dữ liệu không hợp lệ khi tạo dịch vụ");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"Validation Error: {error.ErrorMessage}");
                    }
                }
                viewModel.Buildings = (await _buildingRepository.GetAllBuildingsAsync()).ToList();
                return View(viewModel);
            }

            try
            {
                var service = new Service
                {
                    ServiceName = viewModel.ServiceName,
                    ServiceType = viewModel.ServiceType,
                    PriceType = viewModel.PriceType,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Unit = viewModel.Unit,
                    ServiceCode = viewModel.ServiceCode
                };

                _logger.LogInformation($"Chuẩn bị tạo dịch vụ với mã: {(string.IsNullOrEmpty(service.ServiceCode) ? "Sẽ được tạo tự động" : service.ServiceCode)}");
                
                service = await _serviceRepository.CreateServiceAsync(service);
                
                // Thêm dịch vụ vào các tòa nhà đã chọn
                if (viewModel.SelectedBuildingIds != null && viewModel.SelectedBuildingIds.Any())
                {
                    foreach (var buildingId in viewModel.SelectedBuildingIds)
                    {
                        _logger.LogInformation($"Thêm dịch vụ {service.Id} (Mã: {service.ServiceCode}) vào tòa nhà {buildingId}");
                        await _serviceRepository.AddServiceToBuildingAsync(service.Id, buildingId);
                    }
                }
                
                _logger.LogInformation($"Tạo dịch vụ thành công. ID: {service.Id}, Mã: {service.ServiceCode}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo dịch vụ: {viewModel.ServiceName}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo dịch vụ: " + ex.Message);
                viewModel.Buildings = (await _buildingRepository.GetAllBuildingsAsync()).ToList();
                return View(viewModel);
            }
        }

        // GET: Service/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Truy cập trang chỉnh sửa dịch vụ ID: {id}");
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning($"Không tìm thấy dịch vụ để chỉnh sửa. ID: {id}");
                return NotFound();
            }

            // Lấy danh sách BuildingId từ BuildingServices
            var selectedBuildingIds = service.BuildingServices
                .Where(bs => bs.IsActive)
                .Select(bs => bs.BuildingId)
                .ToList();

            var viewModel = new ServiceViewModel
            {
                Id = service.Id,
                ServiceCode = service.ServiceCode,
                ServiceName = service.ServiceName,
                ServiceType = service.ServiceType,
                PriceType = service.PriceType,
                Description = service.Description,
                Price = service.Price,
                Unit = service.Unit,
                SelectedBuildingIds = selectedBuildingIds,
                Buildings = (await _buildingRepository.GetAllBuildingsAsync()).ToList()
            };

            return View(viewModel);
        }

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceViewModel viewModel)
        {
            _logger.LogInformation($"Bắt đầu cập nhật dịch vụ ID: {id}");
            
            if (id != viewModel.Id)
            {
                _logger.LogWarning($"ID không khớp: {id} != {viewModel.Id}");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dữ liệu không hợp lệ khi cập nhật dịch vụ");
                viewModel.Buildings = (await _buildingRepository.GetAllBuildingsAsync()).ToList();
                return View(viewModel);
            }

            try
            {
                // Cập nhật thông tin dịch vụ
                var service = new Service
                {
                    Id = viewModel.Id,
                    ServiceCode = viewModel.ServiceCode,
                    ServiceName = viewModel.ServiceName,
                    ServiceType = viewModel.ServiceType,
                    PriceType = viewModel.PriceType,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Unit = viewModel.Unit
                };

                await _serviceRepository.UpdateServiceAsync(service);

                // Lấy danh sách tòa nhà hiện tại của dịch vụ
                var currentService = await _serviceRepository.GetServiceByIdAsync(id);
                var currentBuildingServices = currentService.BuildingServices
                    .Where(bs => bs.IsActive)
                    .ToList();

                // Tạo danh sách BuildingId hiện tại và mới
                var currentBuildingIds = currentBuildingServices.Select(bs => bs.BuildingId).ToList();
                var newBuildingIds = viewModel.SelectedBuildingIds ?? new List<int>();

                // Xác định các tòa nhà cần xóa và thêm
                var buildingsToRemove = currentBuildingIds.Except(newBuildingIds).ToList();
                var buildingsToAdd = newBuildingIds.Except(currentBuildingIds).ToList();

                // Xóa các tòa nhà không còn được chọn
                foreach (var buildingId in buildingsToRemove)
                {
                    _logger.LogInformation($"Xóa dịch vụ {id} khỏi tòa nhà {buildingId}");
                    await _serviceRepository.RemoveServiceFromBuildingAsync(id, buildingId);
                }

                // Thêm vào các tòa nhà mới được chọn
                foreach (var buildingId in buildingsToAdd)
                {
                    _logger.LogInformation($"Thêm dịch vụ {id} vào tòa nhà {buildingId}");
                    await _serviceRepository.AddServiceToBuildingAsync(id, buildingId);
                }

                _logger.LogInformation($"Cập nhật dịch vụ thành công. ID: {id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật dịch vụ ID: {id}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật dịch vụ: " + ex.Message);
                viewModel.Buildings = (await _buildingRepository.GetAllBuildingsAsync()).ToList();
                return View(viewModel);
            }
        }

        // GET: Service/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Truy cập trang xóa dịch vụ ID: {id}");
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning($"Không tìm thấy dịch vụ để xóa. ID: {id}");
                return NotFound();
            }
            return View(service);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu xóa dịch vụ ID: {id}");
                await _serviceRepository.DeleteServiceAsync(id);
                _logger.LogInformation($"Xóa dịch vụ thành công. ID: {id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa dịch vụ ID: {id}");
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        // GET: Service/AddToBuilding/5
        public async Task<IActionResult> AddToBuilding(int id)
        {
            _logger.LogInformation($"Truy cập trang thêm dịch vụ {id} vào tòa nhà");
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning($"Không tìm thấy dịch vụ ID: {id}");
                return NotFound();
            }

            var buildings = await _buildingRepository.GetAllBuildingsAsync();
            ViewBag.ServiceId = id;
            ViewBag.Buildings = buildings;
            return View();
        }

        // POST: Service/AddToBuilding/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBuilding(int serviceId, int buildingId)
        {
            try
            {
                _logger.LogInformation($"Thêm dịch vụ {serviceId} vào tòa nhà {buildingId}");
                await _serviceRepository.AddServiceToBuildingAsync(serviceId, buildingId);
                _logger.LogInformation($"Thêm dịch vụ vào tòa nhà thành công");
                return RedirectToAction(nameof(Details), new { id = serviceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi thêm dịch vụ {serviceId} vào tòa nhà {buildingId}");
                return RedirectToAction(nameof(AddToBuilding), new { id = serviceId });
            }
        }

        // POST: Service/RemoveFromBuilding/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromBuilding(int serviceId, int buildingId)
        {
            try
            {
                _logger.LogInformation($"Xóa dịch vụ {serviceId} khỏi tòa nhà {buildingId}");
                await _serviceRepository.RemoveServiceFromBuildingAsync(serviceId, buildingId);
                _logger.LogInformation($"Xóa dịch vụ khỏi tòa nhà thành công");
                return RedirectToAction(nameof(Details), new { id = serviceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa dịch vụ {serviceId} khỏi tòa nhà {buildingId}");
                return RedirectToAction(nameof(Details), new { id = serviceId });
            }
        }
    }
} 