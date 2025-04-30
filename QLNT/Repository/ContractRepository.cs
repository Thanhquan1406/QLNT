using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;
using Microsoft.Extensions.Logging;

namespace QLNT.Repository
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContractRepository> _logger;

        public ContractRepository(ApplicationDbContext context, ILogger<ContractRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implement các phương thức CRUD cơ bản
        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .ToListAsync();
        }

        public async Task<Contract> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Đang tìm hợp đồng với ID: {id}");
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.Customer)
                    .Include(c => c.Room)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (contract == null)
                {
                    _logger.LogWarning($"Không tìm thấy hợp đồng với ID: {id}");
                }
                else
                {
                    _logger.LogInformation($"Đã tìm thấy hợp đồng: ID={contract.Id}, Số hợp đồng={contract.ContractNumber}");
                    if (contract.Customer == null)
                    {
                        _logger.LogWarning($"Hợp đồng {contract.Id} không có thông tin khách hàng");
                    }
                    else
                    {
                        _logger.LogInformation($"Thông tin khách hàng: ID={contract.Customer.Id}, Tên={contract.Customer.FullName }");
                    }
                }

                return contract;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tìm hợp đồng với ID: {id}");
                throw;
            }
        }

        public async Task<Contract> AddAsync(Contract contract)
        {
            try
            {
                Console.WriteLine($"Adding contract to database: {contract.ContractNumber}");
                
                // Kiểm tra xem contract đã có Id chưa
                if (contract.Id != 0)
                {
                    throw new InvalidOperationException("Contract đã có Id, không thể thêm mới");
                }

                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(contract.ContractNumber))
                    throw new ArgumentException("Số hợp đồng không được để trống");
                if (contract.RoomId <= 0)
                    throw new ArgumentException("RoomId không hợp lệ");
                if (contract.CustomerId <= 0)
                    throw new ArgumentException("CustomerId không hợp lệ");
                if (contract.StartDate == default)
                    throw new ArgumentException("Ngày bắt đầu không hợp lệ");
                if (contract.EndDate == default)
                    throw new ArgumentException("Ngày kết thúc không hợp lệ");
                if (contract.SignDate == default)
                    throw new ArgumentException("Ngày ký không hợp lệ");
                if (string.IsNullOrEmpty(contract.Representative))
                    throw new ArgumentException("Người đại diện không được để trống");
                if (contract.RentalPrice <= 0)
                    throw new ArgumentException("Giá thuê phải lớn hơn 0");
                if (string.IsNullOrEmpty(contract.PaymentCycle))
                    throw new ArgumentException("Chu kỳ thanh toán không được để trống");
                if (contract.PaymentStartDate == default)
                    throw new ArgumentException("Ngày bắt đầu thanh toán không hợp lệ");
                if (contract.Deposit < 0)
                    throw new ArgumentException("Tiền cọc không hợp lệ");

                _context.Contracts.Add(contract);
                Console.WriteLine("Contract added to context");

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"Save changes result: {result}");

                return contract;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdateAsync(Contract contract)
        {
            _context.Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }

        // Implement các phương thức tìm kiếm và lọc
        public async Task<IEnumerable<Contract>> GetByBuildingIdAsync(int buildingId)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.Room.BuildingId == buildingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetExpiredContractsAsync()
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.Status == ContractStatus.Expired)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsByStatusAsync(ContractStatus status)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        // Implement các phương thức kiểm tra
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeContractId = null)
        {
            var query = _context.Contracts
                .Where(c => c.RoomId == roomId)
                .Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.AboutToExpire);

            if (excludeContractId.HasValue)
            {
                query = query.Where(c => c.Id != excludeContractId.Value);
            }

            return !await query.AnyAsync(c =>
                (startDate >= c.StartDate && startDate <= c.EndDate) ||
                (endDate >= c.StartDate && endDate <= c.EndDate) ||
                (startDate <= c.StartDate && endDate >= c.EndDate));
        }

        public async Task<bool> HasActiveContractAsync(int roomId)
        {
            return await _context.Contracts
                .AnyAsync(c => c.RoomId == roomId && 
                        (c.Status == ContractStatus.Active || c.Status == ContractStatus.AboutToExpire));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Contracts.AnyAsync(c => c.Id == id);
        }

        // Implement các phương thức đặc thù cho nghiệp vụ hợp đồng
        public async Task<IEnumerable<Contract>> GetContractsExpiringInDaysAsync(int days)
        {
            var expiryDate = DateTime.Now.AddDays(days);
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.Status == ContractStatus.Active && c.EndDate <= expiryDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDepositByCustomerAsync(int customerId)
        {
            return await _context.Contracts
                .Where(c => c.CustomerId == customerId)
                .SumAsync(c => c.Deposit);
        }

        public async Task<IEnumerable<Contract>> GetContractsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.StartDate >= startDate && c.EndDate <= endDate)
                .ToListAsync();
        }

        public async Task<Contract> GetLatestContractByRoomAsync(int roomId)
        {
            return await _context.Contracts
                .Include(c => c.Room)
                .Include(c => c.Customer)
                .Where(c => c.RoomId == roomId)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefaultAsync();
        }

        // Implement các phương thức thống kê
        public async Task<int> GetTotalActiveContractsAsync()
        {
            return await _context.Contracts
                .CountAsync(c => c.Status == ContractStatus.Active);
        }

        public async Task<decimal> GetTotalMonthlyRevenueAsync()
        {
            return await _context.Contracts
                .Where(c => c.Status == ContractStatus.Active)
                .SumAsync(c => c.RentalPrice);
        }

        public async Task<decimal> GetTotalDepositAsync()
        {
            return await _context.Contracts
                .Where(c => c.Status == ContractStatus.Active)
                .SumAsync(c => c.Deposit);
        }

        public async Task<IEnumerable<Contract>> GetContractsByRoomIdAsync(int roomId)
        {
            return await _context.Contracts
                .Where(c => c.RoomId == roomId)
                .ToListAsync();
        }
    }
} 