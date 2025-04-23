using System;
using System.ComponentModel.DataAnnotations;

namespace QLNT.Models
{
    public class ContractViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số hợp đồng")]
        [Display(Name = "Số hợp đồng")]
        public string ContractNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tòa nhà")]
        [Display(Name = "Tòa nhà")]
        public int BuildingId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        [Display(Name = "Phòng")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        [Display(Name = "Khách hàng")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày ký")]
        [Display(Name = "Ngày ký")]
        [DataType(DataType.Date)]
        public DateTime SignDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Hạn hợp đồng")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Mẫu hóa đơn")]
        public string InvoiceTemplate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đại diện")]
        [Display(Name = "Đại diện")]
        public string Representative { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiền thuê")]
        [Display(Name = "Tiền thuê")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền thuê phải lớn hơn 0")]
        public decimal RentalPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chu kỳ thanh toán")]
        [Display(Name = "Chu kỳ thanh toán")]
        public string PaymentCycle { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu thanh toán")]
        [Display(Name = "Ngày bắt đầu thanh toán")]
        [DataType(DataType.Date)]
        public DateTime PaymentStartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiền đặt cọc")]
        [Display(Name = "Tiền đặt cọc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền đặt cọc phải lớn hơn 0")]
        public decimal Deposit { get; set; }

        [Display(Name = "Tiền đã đặt cọc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền đã đặt cọc phải lớn hơn 0")]
        public decimal DepositPaid { get; set; }

        [Display(Name = "Tiền cọc còn phải đóng")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc còn phải đóng phải lớn hơn 0")]
        public decimal DepositRemaining { get; set; }

        [Display(Name = "Số tháng giảm giá")]
        [Range(0, int.MaxValue, ErrorMessage = "Số tháng giảm giá phải lớn hơn 0")]
        public int DiscountMonths { get; set; }

        [Display(Name = "Giảm giá hàng tháng")]
        [Range(0, double.MaxValue, ErrorMessage = "Giảm giá hàng tháng phải lớn hơn 0")]
        public decimal MonthlyDiscount { get; set; }

        [Display(Name = "Trạng thái")]
        public ContractStatus Status { get; set; }
    }
} 