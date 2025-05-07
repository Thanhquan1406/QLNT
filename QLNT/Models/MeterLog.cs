using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNT.Models
{
    public class MeterLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string MeterType { get; set; }

        [Required]
        [StringLength(50)]
        public string MeterName { get; set; }

        [Required]
        public double OldReading { get; set; }

        [Required]
        public double NewReading { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public DateTime ReadingDate { get; set; }

        public bool IsCurrentMeter { get; set; } = false;

        [NotMapped]
        public double Consumption => NewReading - OldReading;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
    }
} 