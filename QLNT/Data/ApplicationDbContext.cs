using Microsoft.EntityFrameworkCore;
using QLNT.Models;

namespace QLNT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        // Thêm các DbSet cho các model của bạn ở đây
        // Ví dụ:
        // public DbSet<YourModel> YourModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
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
        }
    }
} 