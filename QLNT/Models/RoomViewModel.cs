using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tòa nhà")]
        [Display(Name = "Tòa nhà")]
        public int BuildingId { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string BuildingName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tầng")]
        [Display(Name = "Tầng")]
        public string Floor { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        [Display(Name = "Tên phòng")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá thuê")]
        [Display(Name = "Giá thuê")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RentalFee { get; set; }

        [Display(Name = "Trạng thái")]
        public RoomStatus Status { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }
    }
} 