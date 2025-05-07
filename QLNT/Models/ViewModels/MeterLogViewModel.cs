using System.ComponentModel.DataAnnotations;
using QLNT.Models;

namespace QLNT.Models.ViewModels
{
    public class MeterLogViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        [Display(Name = "Phòng")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Loại đồng hồ không được để trống")]
        [Display(Name = "Loại đồng hồ")]
        public string MeterType { get; set; }

        [Required(ErrorMessage = "Chỉ số cũ không được để trống")]
        [Display(Name = "Chỉ số cũ")]
        public double OldReading { get; set; }

        [Required(ErrorMessage = "Chỉ số mới không được để trống")]
        [Display(Name = "Chỉ số mới")]
        public double NewReading { get; set; }

        [Required(ErrorMessage = "Tháng không được để trống")]
        [Display(Name = "Tháng")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Ngày ghi không được để trống")]
        [Display(Name = "Ngày ghi")]
        public DateTime ReadingDate { get; set; }

        public bool IsCurrentMeter { get; set; }

        [Display(Name = "Tòa nhà")]
        public List<Building> Buildings { get; set; }

        [Display(Name = "Phòng")]
        public List<Room> Rooms { get; set; }

        [Display(Name = "Loại đồng hồ")]
        public List<string> MeterTypes { get; set; }

        // Navigation properties
        public Room? Room { get; set; }

        public static MeterLogViewModel FromModel(MeterLog model)
        {
            return new MeterLogViewModel
            {
                Id = model.Id,
                RoomId = model.RoomId,
                MeterType = model.MeterType,
                OldReading = model.OldReading,
                NewReading = model.NewReading,
                Month = model.Month,
                ReadingDate = model.ReadingDate,
                IsCurrentMeter = model.IsCurrentMeter,
                Room = model.Room
            };
        }

        public MeterLog ToModel()
        {
            return new MeterLog
            {
                Id = Id,
                RoomId = RoomId,
                MeterType = MeterType,
                OldReading = OldReading,
                NewReading = NewReading,
                Month = Month,
                ReadingDate = ReadingDate,
                IsCurrentMeter = IsCurrentMeter
            };
        }
    }
} 