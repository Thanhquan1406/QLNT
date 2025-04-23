using System.ComponentModel.DataAnnotations;

namespace QLNT.Models
{
    public class BuildingViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã")]
        public string? Code { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string? Name { get; set; }

        [Display(Name = "Tên viết tắt/Mã tòa")]
        public string? ShortName { get; set; }

        [Display(Name = "Số phòng")]
        public int RoomCount { get; set; }

        [Display(Name = "Địa chỉ chi tiết")]
        public string? Address { get; set; }

        [Display(Name = "Xã/Phường")]
        public string? Ward { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string? Province { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Mẫu hóa đơn")]
        public string? InvoiceTemplate { get; set; }
    }
} 