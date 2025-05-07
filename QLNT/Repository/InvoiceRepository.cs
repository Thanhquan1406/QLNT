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
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> FindAsync(Expression<Func<Invoice, bool>> predicate)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
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
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .Where(i => i.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .Where(i => i.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountByContractIdAsync(int contractId)
        {
            return await _context.Invoices
                .Where(i => i.ContractId == contractId)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<bool> UpdateStatusAsync(int id, InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            invoice.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice> GetLastInvoiceAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.InvoiceId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ApproveInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            invoice.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentAsync(int id, decimal paidAmount)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return false;

            invoice.PaidAmount = paidAmount;
            invoice.Status = paidAmount >= invoice.TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.Pending;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalDebtByContractIdAsync(int contractId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.ContractId == contractId)
                .ToListAsync();

            return invoices.Sum(i => i.CurrentDebt);
        }

        public async Task<IEnumerable<Invoice>> GetApprovedInvoicesAsync(bool isApproved)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                .Include(i => i.Contract)
                    .ThenInclude(c => c.Room)
                        .ThenInclude(r => r.Building)
                .Where(i => i.IsApproved == isApproved)
                .ToListAsync();
        }
    }
} 