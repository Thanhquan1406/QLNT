using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailId { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } // Tên khoản phí (ví dụ: Tiền thuê, Điện, Nước, Dịch vụ...)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Đơn giá

        [Required]
        public decimal Quantity { get; set; } // Số lượng/Đơn vị tính

        [StringLength(50)]
        public string Unit { get; set; } // Đơn vị tính (ví dụ: kWh, m3, tháng...)

        // Các trường cho hóa đơn tiền thuê
        public DateTime? StartDate { get; set; } // Ngày bắt đầu tính phí
        public DateTime? EndDate { get; set; } // Ngày kết thúc tính phí

        // Các trường cho hóa đơn tiền cọc
        [StringLength(50)]
        public string DepositType { get; set; } // Loại tiền cọc (Cọc phòng, Cọc dịch vụ...)
        public bool IsRefundable { get; set; } // Có thể hoàn trả hay không

        [StringLength(500)]
        public string Notes { get; set; } // Ghi chú

        // Tính toán Amount từ UnitPrice và Quantity
        [NotMapped]
        public decimal Amount => UnitPrice * Quantity;
    }
} 