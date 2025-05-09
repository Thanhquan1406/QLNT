using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Models;

namespace QLNT.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var statistics = new StatisticsViewModel
        {
            TotalBuildings = _context.Buildings.Count(),
            TotalRooms = _context.Rooms.Count(),
            TotalCustomers = _context.Customers.Count(),
            TotalContracts = _context.Contracts.Count(),
            TotalInvoices = _context.Invoices.Count(),
            TotalServices = _context.Services.Count(),
            PendingInvoices = _context.Invoices.Count(i => i.IsApproved != true),
            TotalRevenue = _context.Invoices.Where(i => i.IsApproved == true).Sum(i => i.PaidAmount ?? 0)
        };

        return View(statistics);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
