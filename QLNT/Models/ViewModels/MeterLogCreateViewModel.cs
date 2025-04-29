using System;
using System.ComponentModel.DataAnnotations;

namespace QLNT.Models.ViewModels
{
    public class MeterLogCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại đồng hồ")]
        public string MeterType { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên công tơ")]
        [StringLength(50, ErrorMessage = "Tên công tơ không được vượt quá 50 ký tự")]
        public string MeterName { get; set; }

        public double OldReading { get; set; }
        public double NewReading { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tháng")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày ghi")]
        public DateTime ReadingDate { get; set; }
    }
} 