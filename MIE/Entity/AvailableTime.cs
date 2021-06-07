using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIE.Entity
{
    [Table("available_time")]
    public class AvailableTime
    {
        [Column("time_id")]
        [Key]
        public int TimeId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }
    }
}
