using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLNT.Models
{
    public class Customer
    {
        public Customer()
        {
            Contracts = new HashSet<Contract>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        [StringLength(12)]
        public string IdentityCard { get; set; }  // CMND/CCCD

        public DateTime? IdentityCardIssueDate { get; set; }  // Ngày cấp
        
        public string IdentityCardIssuePlace { get; set; }  // Nơi cấp

        public string Province { get; set; }  // Tỉnh/Thành phố
        
        public string District { get; set; }  // Quận/Huyện
        
        public string Ward { get; set; }  // Xã/Phường
        
        public string Address { get; set; }  // Địa chỉ chi tiết

        public string BankAccount { get; set; }  // Số tài khoản
        
        public string BankName { get; set; }  // Ngân hàng

        public string Occupation { get; set; }  // Nghề nghiệp
        
        public string Workplace { get; set; }  // Nơi làm việc

        public string EmergencyContact { get; set; }  // Người liên lạc
        
        public string EmergencyPhone { get; set; }  // SĐT người liên lạc

        public string Consultant { get; set; }  // Người tư vấn
        
        public string ConsultantPhone { get; set; }  // SĐT người tư vấn

        public string AccessCode { get; set; }  // Mã vân tay cửa ra vào

        public bool IsForeigner { get; set; }  // Khách nước ngoài

        public string Note { get; set; }  // Ghi chú

        public string RepresentativeImage { get; set; }  // Ảnh đại diện

        public string FrontIdentityCard { get; set; }  // CCCD mặt trước

        public string BackIdentityCard { get; set; }  // CCCD mặt sau

        public string Portrait { get; set; }  // Hộ chiếu

        // Navigation property
        public virtual ICollection<Contract> Contracts { get; set; }
    }
} 