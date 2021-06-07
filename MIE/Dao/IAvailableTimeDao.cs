using System;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IAvailableTimeDao
    {
        public AvailableTime GetByTime(DateTime startTime, DateTime endTime);
    }
}
