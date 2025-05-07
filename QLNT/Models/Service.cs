using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public enum ServiceTypes
    {
        [Display(Name = "Tiền nhà")]
        RentFee,
        [Display(Name = "Tiền cọc")]
        Deposit,
        [Display(Name = "Tiền điện")]
        Electric,
        [Display(Name = "Tiền nước")]
        Water,
        [Display(Name = "Tiền vệ sinh")]
        Cleaning,
        [Display(Name = "Tiền internet")]
        Internet,
        [Display(Name = "Tiền phí quản lý")]
        Management,
        [Display(Name = "Tiền gửi xe")]
        Parking,
        [Display(Name = "Tiền phí dịch vụ")]
        ServiceFee,
        [Display(Name = "Tiền phí giặt sấy")]
        Laundry
    }

    public enum PriceTypes
    {
        [Display(Name = "Đơn giá cố định theo đồng hộ")]
        FixedPerHousehold,
        [Display(Name = "Đơn giá định mức theo đồng hộ")]
        QuotaPerHousehold,
        [Display(Name = "Đơn giá cố định theo tháng")]
        FixedPerMonth,
        [Display(Name = "Đơn giá biến động")]
        Variable,
        [Display(Name = "Định mức theo số lượng")]
        QuotaByQuantity
    }

    public enum UnitTypes
    {
        [Display(Name = "Người")]
        Person,
        [Display(Name = "Phòng")]
        Room,
        [Display(Name = "Kwh")]
        Kwh,
        [Display(Name = "m³")]
        CubicMeter,
        [Display(Name = "m²")]
        SquareMeter,
        [Display(Name = "Xe")]
        Vehicle,
        [Display(Name = "Lượt/Lần")]
        TimesPerUse,
        [Display(Name = "Kg")]
        Kilogram
    }

    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mã dịch vụ")]
        [StringLength(20)]
        public string ServiceCode { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [Display(Name = "Tên dịch vụ")]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Loại phí không được để trống")]
        [Display(Name = "Loại phí")]
        public ServiceTypes ServiceType { get; set; }

        [Required(ErrorMessage = "Loại đơn giá không được để trống")]
        [Display(Name = "Loại đơn giá")]
        public PriceTypes PriceType { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Display(Name = "Đơn giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        [Display(Name = "Đơn vị tính")]
        public UnitTypes Unit { get; set; }

        // Quan hệ nhiều-nhiều với Building thông qua BuildingService
        public virtual ICollection<BuildingService> BuildingServices { get; set; } = new List<BuildingService>();

        // Quan hệ nhiều-nhiều với Room thông qua RoomService
        public virtual ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();

        // Navigation property để truy cập các Building thông qua BuildingService
        [NotMapped]
        public virtual ICollection<Building> Buildings => BuildingServices.Select(bs => bs.Building).ToList();
    }

    // Lớp trung gian để quản lý quan hệ nhiều-nhiều
    public class BuildingService
    {
        public int BuildingId { get; set; }
        public virtual Building Building { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        // Các thuộc tính bổ sung cho quan hệ
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
} 