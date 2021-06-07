using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao.Impl
{
    public class ReservationDaoImpl : IReservationDao
    {
        private readonly MySQLDbContext context;

        public ReservationDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public bool AddReservation(Reservation reservation)
        {
            context.Reservation.Add(reservation);
            return context.SaveChanges() > 0;
        }

        public List<Reservation> GetReservations(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
