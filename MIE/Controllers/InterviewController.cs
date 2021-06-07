using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Dto;
using MIE.Entity;
using MIE.Utils;

namespace MIE.Controllers
{
    [Route("/interview")]
    public class InterviewController : Controller
    {
        private readonly IAvailableTimeDao availableTimeDao;
        private readonly IAuthUtil authUtil;
        private readonly IReservationDao reservationDao;

        public InterviewController(IAvailableTimeDao availableTimeDao, IAuthUtil authUtil, IReservationDao reservationDao)
        {
            this.availableTimeDao = availableTimeDao;
            this.authUtil = authUtil;
            this.reservationDao = reservationDao;
        }
        

        [HttpPost]
        [Authorize]
        public string GetMyInterverw()
        {
            int userId = authUtil.GetIdFromToken();
            return "";
            // reservationDao
        }

        
        [HttpPost]
        [Authorize]
        public IActionResult PostReservation([FromBody] ReservationDto reservationDto)
        {
            AvailableTime time = availableTimeDao.GetByTime(reservationDto.StartTime, reservationDto.EndTime);
            if (time == null) return NotFound("无可用预约时间");
            int userId = authUtil.GetIdFromToken();
            //TODO: 推荐题目
            int quizId = new Random().Next(50);
            Reservation tar = new Reservation(time.TimeId, quizId, userId, reservationDto.ReserveDate);
            return Ok(new { result = reservationDao.AddReservation(tar) });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
