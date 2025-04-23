using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mã")]
        [Required(ErrorMessage = "Vui lòng nhập mã")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tòa nhà")]
        [Display(Name = "Tên tòa nhà")]
        public string Name { get; set; }

        [Display(Name = "Tên viết tắt/Mã tòa")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ chi tiết")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn xã/phường")]
        [Display(Name = "Xã/Phường")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string Province { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Mẫu hóa đơn")]
        public string InvoiceTemplate { get; set; }

        // Navigation property
        [InverseProperty("Building")]
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
} 