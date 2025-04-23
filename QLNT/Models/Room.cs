using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QLNT.Models
{
    [Table("Rooms")]
    public class Room
    {
        public Room()
        {
            Contracts = new HashSet<Contract>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã phòng không được để trống")]
        [StringLength(20, ErrorMessage = "Mã phòng không được vượt quá 20 ký tự")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tòa nhà")]
        [Display(Name = "Tòa nhà")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn tòa nhà")]
        public int BuildingId { get; set; }

        [ForeignKey("BuildingId")]
        [Display(Name = "Tòa nhà")]
        [ValidateNever]
        public virtual Building Building { get; set; }

        [Required(ErrorMessage = "Tầng không được để trống")]
        [StringLength(10, ErrorMessage = "Tầng không được vượt quá 10 ký tự")]
        public string Floor { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên phòng không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá thuê không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RentalFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deposit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Area { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public RoomStatus Status { get; set; }

        public bool IsActive { get; set; } = true;

        public string? InvoiceTemplate { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }

    public enum RoomStatus
    {
        [Display(Name = "Trống")]
        Available = 1,
        [Display(Name = "Đã thuê")]
        Rented = 2,
        [Display(Name = "Đang sửa chữa")]
        UnderMaintenance = 3,
        [Display(Name = "Tạm ngưng")]
        Suspended = 4
    }
} 