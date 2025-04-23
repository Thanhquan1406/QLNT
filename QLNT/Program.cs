using Microsoft.EntityFrameworkCore;
using QLNT.Data;
using QLNT.Services;
using QLNT.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using QLNT.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Thêm dịch vụ vào container.
builder.Services.AddControllersWithViews();

// Cấu hình cho phép upload file lớn hơn
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
});

try
{
    // Thêm DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        
        // Tắt sensitive data logging trong môi trường production
        if (!builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging(false);
        }
    });

    // Thêm dịch vụ tùy chỉnh
    builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
    builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IContractRepository, ContractRepository>();

    var app = builder.Build();

    // Cấu hình pipeline xử lý HTTP request.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        // Tắt HTTPS redirection trong môi trường development
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Lỗi khởi tạo ứng dụng: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}
