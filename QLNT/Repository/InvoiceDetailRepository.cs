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
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDetail> GetByIdAsync(int invoiceDetailId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Invoice)
                .FirstOrDefaultAsync(detail => detail.InvoiceDetailId == invoiceDetailId);
        }

        public async Task<IEnumerable<InvoiceDetail>> GetAllAsync()
        {
            return await _context.InvoiceDetails
                .Include(id => id.Invoice)
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceDetail>> FindAsync(Expression<Func<InvoiceDetail, bool>> predicate)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Invoice)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<InvoiceDetail> AddAsync(InvoiceDetail entity)
        {
            await _context.InvoiceDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<InvoiceDetail> UpdateAsync(InvoiceDetail entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detail = await _context.InvoiceDetails.FindAsync(id);
            if (detail == null)
                return false;

            _context.InvoiceDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.InvoiceDetails.AnyAsync(e => e.InvoiceDetailId == id);
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Include(id => id.Invoice)
                .Where(detail => detail.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Where(detail => detail.InvoiceId == invoiceId)
                .SumAsync(detail => detail.Amount);
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByServiceIdAsync(int serviceId)
        {
            return await _context.InvoiceDetails
                .Include(detail => detail.Invoice)
                .Where(detail => detail.Invoice.Contract.Room.RoomServices.Any(rs => rs.ServiceId == serviceId))
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceDetail>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.InvoiceDetails
                .Include(detail => detail.Invoice)
                .Where(detail => detail.Invoice.IssueDate >= startDate && detail.Invoice.IssueDate <= endDate)
                .ToListAsync();
        }
    }
} 