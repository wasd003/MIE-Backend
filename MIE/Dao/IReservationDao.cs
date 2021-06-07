using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IReservationDao
    {
        bool AddReservation(Reservation reservation);

        List<Reservation> GetReservations(int userId);
    }
}
