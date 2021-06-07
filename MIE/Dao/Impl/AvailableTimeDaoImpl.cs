using System;
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
        {
            return context.AvailableTime.FirstOrDefault(cur => cur.StartTime.Equals(startTime) && cur.EndTime.Equals(endTime));
        }
    }
}
