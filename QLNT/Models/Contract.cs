using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Số hợp đồng")]
        public string ContractNumber { get; set; }

        [Required]
        [Display(Name = "Ngày ký")]
        public DateTime SignDate { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Mẫu hóa đơn")]
        public string InvoiceTemplate { get; set; }

        [Required]
        [Display(Name = "Đại diện")]
        public string Representative { get; set; }

        [Required]
        [Display(Name = "Tiền thuê")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RentalPrice { get; set; }

        [Required]
        [Display(Name = "Chu kỳ thanh toán")]
        public string PaymentCycle { get; set; }

        [Required]
        [Display(Name = "Ngày bắt đầu tính tiền")]
        public DateTime PaymentStartDate { get; set; }

        [Required]
        [Display(Name = "Tiền cọc")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deposit { get; set; }

        [Display(Name = "Đã đặt cọc")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositPaid { get; set; }

        [Display(Name = "Tiền cọc phải đóng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositRemaining { get; set; }

        [Display(Name = "Số tháng giảm")]
        public int DiscountMonths { get; set; }

        [Display(Name = "Số tiền giảm mỗi tháng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyDiscount { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public ContractStatus Status { get; set; }

        // Foreign keys
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public enum ContractStatus
    {
        [Display(Name = "Hiệu lực")]
        Active = 1,
        [Display(Name = "Sắp hết hạn")]
        AboutToExpire = 2,
        [Display(Name = "Quá hạn")]
        Expired = 3,
        [Display(Name = "Đã thanh lý")]
        Terminated = 4
    }
} 