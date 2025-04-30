using QLNT.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> FindAsync(Expression<Func<Invoice, bool>> predicate);
        Task<Invoice> AddAsync(Invoice entity);
        Task<Invoice> UpdateAsync(Invoice entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Invoice>> GetByContractIdAsync(int contractId);
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status);
        Task<IEnumerable<Invoice>> GetByTypeAsync(InvoiceType type);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalAmountByContractIdAsync(int contractId);
        Task<decimal> GetTotalAmountByCustomerIdAsync(int customerId);
        Task<bool> UpdateStatusAsync(int id, InvoiceStatus status, DateTime? paidDate = null);
        Task<Invoice> GetLastInvoiceAsync();
    }
} 