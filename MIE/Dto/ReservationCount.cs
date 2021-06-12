using System;
namespace MIE.Dto
{
    public class ReservationCount : IComparable<ReservationCount>
    {
        public int TimeId { get; set; }

        public string StartTime { get; set; }

        public string Date { get; set; }

        public int Count { get; set; }

        private DateTime dateTime;

        public ReservationCount(DateTime dateTime, int count, int timeId)
        {
            this.dateTime = dateTime;
            this.StartTime = dateTime.ToLongTimeString();
            this.Date = dateTime.ToShortDateString();
            this.Count = count;
            this.TimeId = timeId;
        }

        public int CompareTo(ReservationCount rhs)
        {
            return dateTime.CompareTo(rhs.dateTime);
        }
    }
}
