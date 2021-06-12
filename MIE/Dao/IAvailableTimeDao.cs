using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IAvailableTimeDao
    {
        AvailableTime GetByTime(DateTime startTime, DateTime endTime);

        AvailableTime GetByTimeId(int timeId);

        List<AvailableTime> GetAllTime();
    }
}
