using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã tòa nhà không được để trống")]
        [Display(Name = "Mã tòa nhà")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên tòa nhà không được để trống")]
        [Display(Name = "Tên tòa nhà")]
        public string Name { get; set; }

        [Display(Name = "Tên viết tắt")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phường/Xã không được để trống")]
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Quận/Huyện không được để trống")]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Tỉnh/Thành phố không được để trống")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string Province { get; set; }

        [Display(Name = "Mẫu hóa đơn")]
        public string InvoiceTemplate { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }

        // Navigation properties
        [InverseProperty("Building")]
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        
        // Quan hệ nhiều-nhiều với Service thông qua BuildingService
        public virtual ICollection<BuildingService> BuildingServices { get; set; } = new List<BuildingService>();

        // Navigation property để truy cập các Service thông qua BuildingService
        [NotMapped]
        public virtual ICollection<Service> Services => BuildingServices.Select(bs => bs.Service).ToList();
    }
} 