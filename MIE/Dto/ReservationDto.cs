using System;
using MIE.Entity;

namespace MIE.Dto
{
    public class ReservationDto
    {
        public ReservationDto() { }

        public ReservationDto(Reservation r, int iamB)
        {
            ReserveDate = r.ReserveDate.ToShortDateString();
            StartTime = r.StartTime.ToLongTimeString();
            EndTime = r.EndTime.ToLongTimeString();
            int[] id = { r.UserAId, r.UserBId, r.QuizAId, r.QuizBId };
            string[] names = { r.UserAName, r.UserBName, r.QuizAName, r.QuizBName };
            MyId = id[iamB]; CompanionId = id[iamB ^ 1];
            MyQuizId = id[2 + iamB]; CompanionQuizId = id[2 + (iamB ^ 1)];
            MyName = names[iamB]; CompanionName = names[iamB ^ 1];
            MyQuizName = names[2 + iamB]; CompanionQuizName = names[2 + (iamB ^ 1)];
        }

        public string ReserveDate { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int MyId { get; set; }
        public string MyName { get; set; }

        public int CompanionId { get; set; }
        public string CompanionName { get; set; }

        public int MyQuizId { get; set; }
        public string MyQuizName { get; set; }

        public int CompanionQuizId { get; set; }
        public string CompanionQuizName { get; set; }

        public static ReservationDto ToDto(Reservation reservation, bool iamB)
            => new ReservationDto(reservation, iamB? 1 : 0);

    }
}
