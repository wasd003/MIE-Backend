using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIE.Entity
{
    [Table("reservation")]
    public class Reservation
    {
        public Reservation(AvailableTime time, Quiz quiza, Quiz quizb,
            User usera, User userb, DateTime reserveDate) :
            this(time.TimeId, time.StartTime, time.EndTime, quiza.QuizId, quiza.QuizName,
                quizb.QuizId, quizb.QuizName, usera.UserId, usera.Username,
                userb.UserId, userb.Username, reserveDate)
        {

        }
        public Reservation(int timeId, DateTime startTime, DateTime endTime,
            int quizAId, string quizAName, int quizBId, string quizBName,
            int userAId, string userAName, int userBId, string userBName, DateTime reserveDate)
        {
            TimeId = timeId;
            StartTime = startTime;
            EndTime = endTime;
            QuizAId = quizAId;
            QuizAName = quizAName;
            QuizBId = quizBId;
            QuizBName = quizBName;
            UserAId = userAId;
            UserAName = userAName;
            UserBId = userBId;
            UserBName = userBName;
            ReserveDate = reserveDate;
        }

        [Column("reservation_id")]
        public int ReservationId { get; set; }

        [Column("time_id")]
        public int TimeId { get; set; }
        public AvailableTime Time { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("quiza_id")]
        public int QuizAId { get; set; }
        public Quiz QuizA { get; set; }

        [Column("quiza_name")]
        public string QuizAName { get; set; }

        [Column("quizb_id")]
        public int QuizBId { get; set; }
        public Quiz QuizB { get; set; }

        [Column("quizb_name")]
        public string QuizBName { get; set; }

        [Column("usera_id")]
        public int UserAId { get; set; }
        public User UserA { get; set; }

        [Column("usera_username")]
        public string UserAName { get; set; }

        [Column("userb_id")]
        public int UserBId { get; set; }
        public User UserB { get; set; }

        [Column("userb_username")]
        public string UserBName { get; set; }

        [Column("reserve_date")]
        public DateTime ReserveDate { get; set; }

    }
}
