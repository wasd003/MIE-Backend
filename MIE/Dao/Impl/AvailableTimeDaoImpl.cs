using System;
using System.Collections.Generic;
using System.Linq;
using MIE.Entity;

namespace MIE.Dao.Impl
{
    public class AvailableTimeDaoImpl : IAvailableTimeDao
    {
        private readonly MySQLDbContext context;

        public AvailableTimeDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public AvailableTime GetByTime(DateTime startTime, DateTime endTime)
            => context.AvailableTime.FirstOrDefault(
            cur => cur.StartTime.Equals(startTime) && cur.EndTime.Equals(endTime));


        public AvailableTime GetByTimeId(int timeId)
            => context.AvailableTime.FirstOrDefault(cur => cur.TimeId == timeId);

        public List<AvailableTime> GetAllTime()
            => context.AvailableTime.Where(i => true).ToList();
    }
}
