using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLNT.Models;

namespace QLNT.Repository
{
    public interface IContractRepository
    {
        // Các phương thức CRUD cơ bản
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract> GetByIdAsync(int id);
        Task<Contract> AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);

        // Các phương thức tìm kiếm và lọc
        Task<IEnumerable<Contract>> GetByBuildingIdAsync(int buildingId);
        Task<IEnumerable<Contract>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<Contract>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Contract>> GetActiveContractsAsync();
        Task<IEnumerable<Contract>> GetExpiredContractsAsync();
        Task<IEnumerable<Contract>> GetContractsByStatusAsync(ContractStatus status);
        
        // Các phương thức kiểm tra
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeContractId = null);
        Task<bool> HasActiveContractAsync(int roomId);
        Task<bool> ExistsAsync(int id);

        // Các phương thức đặc thù cho nghiệp vụ hợp đồng
        Task<IEnumerable<Contract>> GetContractsExpiringInDaysAsync(int days);
        Task<decimal> GetTotalDepositByCustomerAsync(int customerId);
        Task<IEnumerable<Contract>> GetContractsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Contract> GetLatestContractByRoomAsync(int roomId);
        
        // Các phương thức thống kê
        Task<int> GetTotalActiveContractsAsync();
        Task<decimal> GetTotalMonthlyRevenueAsync();
        Task<decimal> GetTotalDepositAsync();

        Task<IEnumerable<Contract>> GetContractsByRoomIdAsync(int roomId);
    }
} 