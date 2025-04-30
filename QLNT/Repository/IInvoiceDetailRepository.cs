using QLNT.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public interface IInvoiceDetailRepository
    {
        Task<InvoiceDetail> GetByIdAsync(int id);
        Task<IEnumerable<InvoiceDetail>> GetAllAsync();
        Task<IEnumerable<InvoiceDetail>> FindAsync(Expression<Func<InvoiceDetail, bool>> predicate);
        Task<InvoiceDetail> AddAsync(InvoiceDetail entity);
        Task<InvoiceDetail> UpdateAsync(InvoiceDetail entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<InvoiceDetail>> GetByInvoiceIdAsync(int invoiceId);
        Task<decimal> GetTotalAmountByInvoiceIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceDetail>> GetByServiceIdAsync(int serviceId);
        Task<IEnumerable<InvoiceDetail>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 