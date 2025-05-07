using System;

namespace QLNT.Models
{
    public class StatisticsViewModel
    {
        public int TotalBuildings { get; set; }
        public int TotalRooms { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalContracts { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalServices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
    }
} 