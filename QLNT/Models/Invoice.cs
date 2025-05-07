using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using QLNT.Models;

namespace QLNT.Models
{
    public enum InvoiceType
    {
        [Display(Name = "Hóa đơn dịch vụ")]
        Service,
        [Display(Name = "Hóa đơn lệ phí")]
        RoomRent
    }

    public enum InvoiceStatus
    {
        [Display(Name = "Chưa thanh toán")]
        Pending,
        [Display(Name = "Đã thanh toán")]
        Paid,
        [Display(Name = "Quá hạn")]
        Overdue,
        [Display(Name = "Đã hủy")]
        Cancelled
    }

    public class Invoice
    {
        public Invoice()
        {
            InvoiceNumber = string.Empty;
            PaymentCycle = string.Empty;
            Notes = string.Empty;
            InvoiceDetails = new List<InvoiceDetail>();
        }

        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã hóa đơn")]
        public string InvoiceNumber { get; set; }

        [Required]
        [Display(Name = "Hợp đồng")]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [Required]
        [Display(Name = "Loại hóa đơn")]
        public InvoiceType InvoiceType { get; set; }

        [Required]
        [Display(Name = "Kỳ thanh toán")]
        [StringLength(50)]
        public string PaymentCycle { get; set; }

        [Required]
        [Display(Name = "Ngày lập")]
        public DateTime IssueDate { get; set; }

        [Required]
        [Display(Name = "Hạn thanh toán")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

        [Display(Name = "Tiền thuê")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RentAmount { get; set; }

        [Display(Name = "Tiền dịch vụ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ServiceAmount { get; set; }

        [Display(Name = "Giảm giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Đã thanh toán")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }

        [Display(Name = "Nợ cộng dồn")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalDebt { get; set; }

        [Display(Name = "Đã duyệt")]
        public bool? IsApproved { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string Notes { get; set; }

        // Collection các chi tiết hóa đơn
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        // Phương thức để tính khoản nợ hiện tại
        [NotMapped]
        [Display(Name = "Nợ")]
        public decimal CurrentDebt => TotalAmount - (PaidAmount ?? 0);

        // Phương thức để cập nhật thông tin từ hợp đồng
        public void UpdateFromContract(Contract? contract)
        {
            if (contract != null)
            {
                ContractId = contract.Id;
                PaymentCycle = contract.PaymentCycle;
                RentAmount = contract.RentalPrice;
            }
        }
    }
} 