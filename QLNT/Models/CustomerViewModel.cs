using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace QLNT.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "CMND/CCCD")]
        [StringLength(12)]
        public string IdentityCard { get; set; }

        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        public DateTime? IdentityCardIssueDate { get; set; }

        [Display(Name = "Nơi cấp")]
        public string IdentityCardIssuePlace { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string Province { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Display(Name = "Xã/Phường")]
        public string Ward { get; set; }

        [Display(Name = "Địa chỉ chi tiết")]
        public string Address { get; set; }

        [Display(Name = "Số tài khoản")]
        public string BankAccount { get; set; }

        [Display(Name = "Ngân hàng")]
        public string BankName { get; set; }

        [Display(Name = "Nghề nghiệp")]
        public string Occupation { get; set; }

        [Display(Name = "Nơi làm việc")]
        public string Workplace { get; set; }

        [Display(Name = "Người liên lạc")]
        public string EmergencyContact { get; set; }

        [Display(Name = "SĐT người liên lạc")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string EmergencyPhone { get; set; }

        [Display(Name = "Người tư vấn")]
        public string Consultant { get; set; }

        [Display(Name = "SĐT người tư vấn")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string ConsultantPhone { get; set; }

        [Display(Name = "Mã vân tay cửa ra vào")]
        public string AccessCode { get; set; }

        [Display(Name = "Khách nước ngoài")]
        public bool IsForeigner { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "File ảnh đại diện")]
        public IFormFile RepresentativeImageFile { get; set; }

        [Display(Name = "File CCCD mặt trước")]
        public IFormFile FrontIdentityCardFile { get; set; }

        [Display(Name = "File CCCD mặt sau")]
        public IFormFile BackIdentityCardFile { get; set; }

        [Display(Name = "File hộ chiếu")]
        public IFormFile PortraitFile { get; set; }

        public string RepresentativeImage { get; set; }
        public string FrontIdentityCard { get; set; }
        public string BackIdentityCard { get; set; }
        public string Portrait { get; set; }
    }
} 