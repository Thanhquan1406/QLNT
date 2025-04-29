using System.ComponentModel.DataAnnotations;

namespace QLNT.Models
{
    public class ServiceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã dịch vụ")]
        [StringLength(20)]
        public string? ServiceCode { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [Display(Name = "Tên dịch vụ")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Loại phí không được để trống")]
        [Display(Name = "Loại phí")]
        public ServiceTypes ServiceType { get; set; }

        [Required(ErrorMessage = "Loại đơn giá không được để trống")]
        [Display(Name = "Loại đơn giá")]
        public PriceTypes PriceType { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Tên tòa nhà")]
        public string? BuildingName { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        [Display(Name = "Đơn vị tính")]
        public UnitTypes Unit { get; set; }

        [Display(Name = "Tòa nhà sử dụng")]
        public List<int> SelectedBuildingIds { get; set; } = new List<int>();

        public List<Building>? Buildings { get; set; }
    }
} 