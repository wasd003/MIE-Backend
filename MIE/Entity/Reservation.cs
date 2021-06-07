using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIE.Entity
{
    [Table("reservation")]
    public class Reservation
    {
        public Reservation(int timeId, int quizId, int userId, DateTime reserveDate)
        {
            this.TimeId = timeId;
            this.QuizId = quizId;
            this.UserId = userId;
            this.ReserveDate = reserveDate;
        }

        [Column("reservation_id")]
        public int ReservationId { get; set; }

        [Column("time_id")]
        public int TimeId { get; set; }
        public AvailableTime Time { get; set; }

        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("reserve_date")]
        public DateTime ReserveDate { get; set; }

    }
}
