using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QLNT.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> FindAsync(Expression<Func<Invoice, bool>> predicate)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Invoice> AddAsync(Invoice entity)
        {
            await _context.Invoices.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Invoice> UpdateAsync(Invoice entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Invoices.AnyAsync(e => e.InvoiceId == id);
        }

        public async Task<IEnumerable<Invoice>> GetByContractIdAsync(int contractId)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.Contract.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByTypeAsync(InvoiceType type)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceType == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Include(i => i.Contract)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountByContractIdAsync(int contractId)
        {
            return await _context.Invoices
                .Where(i => i.ContractId == contractId)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<decimal> GetTotalAmountByCustomerIdAsync(int customerId)
        {
            return await _context.Invoices
                .Where(i => i.Contract.CustomerId == customerId)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus status, DateTime? paidDate = null)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            invoice.Status = status;
            if (paidDate.HasValue)
                invoice.PaidDate = paidDate.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice> GetLastInvoiceAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefaultAsync();
        }
    }
} 