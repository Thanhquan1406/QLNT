using QLNT.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IInvoiceRepository
    {
        // Lấy hóa đơn theo ID với đầy đủ thông tin liên quan
        Task<Invoice> GetByIdAsync(int id);
        
        // Lấy tất cả hóa đơn
        Task<IEnumerable<Invoice>> GetAllAsync();
        
        // Tìm kiếm hóa đơn theo điều kiện
        Task<IEnumerable<Invoice>> FindAsync(Expression<Func<Invoice, bool>> predicate);
        
        // Thêm hóa đơn mới
        Task<Invoice> AddAsync(Invoice entity);
        
        // Cập nhật hóa đơn
        Task<Invoice> UpdateAsync(Invoice entity);
        
        // Xóa hóa đơn
        Task<bool> DeleteAsync(int id);
        
        // Kiểm tra hóa đơn có tồn tại không
        Task<bool> ExistsAsync(int id);
        
        // Lấy danh sách hóa đơn theo hợp đồng
        Task<IEnumerable<Invoice>> GetByContractIdAsync(int contractId);
        
        // Lấy danh sách hóa đơn theo trạng thái
        Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status);
        
        // Lấy danh sách hóa đơn trong khoảng thời gian
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        // Tính tổng tiền theo hợp đồng
        Task<decimal> GetTotalAmountByContractIdAsync(int contractId);
        
        // Cập nhật trạng thái hóa đơn
        Task<bool> UpdateStatusAsync(int id, InvoiceStatus status);
        
        // Lấy hóa đơn gần nhất
        Task<Invoice> GetLastInvoiceAsync();
        
        // Duyệt hóa đơn
        Task<bool> ApproveInvoiceAsync(int id);
        
        // Cập nhật thanh toán
        Task<bool> UpdatePaymentAsync(int id, decimal paidAmount);
        
        // Tính tổng nợ theo hợp đồng
        Task<decimal> GetTotalDebtByContractIdAsync(int contractId);
        
        // Lấy danh sách hóa đơn đã duyệt/chưa duyệt
        Task<IEnumerable<Invoice>> GetApprovedInvoicesAsync(bool isApproved);
    }
} 