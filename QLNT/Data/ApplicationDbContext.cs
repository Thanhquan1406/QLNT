using Microsoft.EntityFrameworkCore;
using QLNT.Models;
using QLNT.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QLNT.Models.Identity;

namespace QLNT.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Xóa toàn bộ phần log SQL
            optionsBuilder.LogTo(message => {
                // Không cần log gì cả
            }, LogLevel.Information);
        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BuildingService> BuildingServices { get; set; }
        public DbSet<MeterLog> MeterLogs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Cấu hình Identity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            // Cấu hình Service
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ServiceType).IsRequired();
                entity.Property(e => e.PriceType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
            });

            // Cấu hình BuildingService (bảng trung gian)
            modelBuilder.Entity<BuildingService>(entity =>
            {
                entity.HasKey(bs => new { bs.BuildingId, bs.ServiceId });

                entity.HasOne(bs => bs.Building)
                    .WithMany(b => b.BuildingServices)
                    .HasForeignKey(bs => bs.BuildingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.Service)
                    .WithMany(s => s.BuildingServices)
                    .HasForeignKey(bs => bs.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
                
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            // Cấu hình Building
            modelBuilder.Entity<Building>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ShortName).HasMaxLength(50);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Ward).IsRequired().HasMaxLength(100);
                entity.Property(e => e.District).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Province).IsRequired().HasMaxLength(100);
                entity.Property(e => e.InvoiceTemplate).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Cấu hình Room
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Floor).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RentalFee).HasPrecision(18, 2);
                entity.Property(e => e.Deposit).HasPrecision(18, 2);
                entity.Property(e => e.Area).HasPrecision(10, 2);
                entity.Property(e => e.InvoiceTemplate).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Status)
                    .HasDefaultValue(RoomStatus.Available)
                    .HasSentinel(RoomStatus.Available);

                // Cấu hình quan hệ với Building
                entity.HasOne(r => r.Building)
                    .WithMany(b => b.Rooms)
                    .HasForeignKey(r => r.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.IdentityCard).HasMaxLength(12);
                entity.Property(e => e.IdentityCardIssuePlace).HasMaxLength(200);
                entity.Property(e => e.Province).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.Ward).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.BankAccount).HasMaxLength(50);
                entity.Property(e => e.BankName).HasMaxLength(200);
                entity.Property(e => e.Occupation).HasMaxLength(200);
                entity.Property(e => e.Workplace).HasMaxLength(500);
                entity.Property(e => e.EmergencyContact).HasMaxLength(100);
                entity.Property(e => e.EmergencyPhone).HasMaxLength(10);
                entity.Property(e => e.Consultant).HasMaxLength(100);
                entity.Property(e => e.ConsultantPhone).HasMaxLength(10);
                entity.Property(e => e.AccessCode).HasMaxLength(50);
                entity.Property(e => e.Note).HasMaxLength(1000);
                
                // Cấu hình các thuộc tính lưu đường dẫn ảnh
                entity.Property(e => e.RepresentativeImage).HasMaxLength(500);
                entity.Property(e => e.FrontIdentityCard).HasMaxLength(500);
                entity.Property(e => e.BackIdentityCard).HasMaxLength(500);
                entity.Property(e => e.Portrait).HasMaxLength(500);
            });

            // Cấu hình Contract
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.InvoiceTemplate).HasMaxLength(1000);
                entity.Property(e => e.Representative).IsRequired().HasMaxLength(200);
                entity.Property(e => e.RentalPrice).HasPrecision(18, 2);
                entity.Property(e => e.PaymentCycle).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Deposit).HasPrecision(18, 2);
                entity.Property(e => e.DepositPaid).HasPrecision(18, 2);
                entity.Property(e => e.DepositRemaining).HasPrecision(18, 2);
                entity.Property(e => e.MonthlyDiscount).HasPrecision(18, 2);
                entity.Property(e => e.Status)
                    .HasDefaultValue(ContractStatus.Active)
                    .HasSentinel(ContractStatus.Active);
            });

            // Cấu hình quan hệ giữa Contract và Room
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Room)
                .WithMany(r => r.Contracts)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ giữa Contract và Customer
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Customer)
                .WithMany(cu => cu.Contracts)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ giữa Room và Building
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình MeterLog
            modelBuilder.Entity<MeterLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MeterType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MeterName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Month).IsRequired().HasMaxLength(20);
                entity.Property(e => e.OldReading).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.NewReading).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.ReadingDate).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsCurrentMeter).HasDefaultValue(false);

                // Cấu hình quan hệ với Room
                entity.HasOne(ml => ml.Room)
                    .WithMany(r => r.MeterLogs)
                    .HasForeignKey(ml => ml.RoomId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PaymentCycle).HasMaxLength(50);
                entity.Property(e => e.RentAmount).HasPrecision(18, 2);
                entity.Property(e => e.ServiceAmount).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalDebt).HasPrecision(18, 2);
                entity.Property(e => e.Notes).HasMaxLength(500).IsRequired(false);

                // Cấu hình quan hệ với Contract
                entity.HasOne(i => i.Contract)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(i => i.ContractId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                // Cấu hình quan hệ với InvoiceDetails
                entity.HasMany(i => i.InvoiceDetails)
                    .WithOne(id => id.Invoice)
                    .HasForeignKey(id => id.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình InvoiceDetail
            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.InvoiceDetailId);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.Quantity).HasPrecision(18, 2);
                entity.Property(e => e.Unit).HasMaxLength(50);
                entity.Property(e => e.DepositType).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Cấu hình quan hệ với Service
                entity.HasOne(id => id.Service)
                    .WithMany()
                    .HasForeignKey(id => id.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình RoomService
            modelBuilder.Entity<RoomService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                // Cấu hình quan hệ với Room
                entity.HasOne(rs => rs.Room)
                    .WithMany(r => r.RoomServices)
                    .HasForeignKey(rs => rs.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Cấu hình quan hệ với Service
                entity.HasOne(rs => rs.Service)
                    .WithMany(s => s.RoomServices)
                    .HasForeignKey(rs => rs.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 