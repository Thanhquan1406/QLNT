using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(10)]
        public string InvoiceNumber { get; set; }

        [Required]
        public int ContractId { get; set; }
        [ForeignKey("ContractId")]

        
        public virtual Contract Contract { get; set; }

        [Required]
        public InvoiceType InvoiceType { get; set; }

        [Required]
        [StringLength(20)]
        public string Period { get; set; } // Ví dụ: "Tháng 4/2024" hoặc "Kỳ 1"

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; }

        public DateTime? PaidDate { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; } // Số tham chiếu thanh toán

        [StringLength(500)]
        public string Notes { get; set; }

        // Navigation property để truy cập thông tin người thuê thông qua hợp đồng
        [NotMapped]
        public Customer Customer => Contract?.Customer;

        // Collection các chi tiết hóa đơn
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        // Tính tổng tiền từ các chi tiết hóa đơn
        [NotMapped]
        public decimal TotalAmount => InvoiceDetails?.Sum(d => d.Amount) ?? 0;
    }
} 