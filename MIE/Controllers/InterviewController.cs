using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Dto;
using MIE.Entity;
using MIE.Utils;
using StackExchange.Redis;
using MIE.Entity.Enum;
using MIE.Service;
using System.Threading.Tasks;

namespace MIE.Controllers
{
    [Route("/interview")]
    public class InterviewController : Controller
    {
        private readonly IAvailableTimeDao availableTimeDao;
        private readonly IAuthUtil authUtil;
        private readonly IReservationDao reservationDao;
        private readonly IQuizDao quizDao;
        private readonly IUserDao userDao;
        private readonly IDatabase redis;
        private readonly IRecommend recommendService;

        public InterviewController(IAvailableTimeDao availableTimeDao, IAuthUtil authUtil,
            IReservationDao reservationDao, IQuizDao quizDao,
            IUserDao userDao, IConnectionMultiplexer connectionMultiplexer, IRecommend recommend)
        {
            this.availableTimeDao = availableTimeDao;
            this.authUtil = authUtil;
            this.reservationDao = reservationDao;
            this.quizDao = quizDao;
            this.userDao = userDao;
            this.redis = connectionMultiplexer.GetDatabase();
            recommendService = recommend;
        }

        [HttpPost("api/match")]
        [Authorize]
        public async Task<IActionResult> MatchAsync([FromBody] PostReservationDto reservationDto)
        {
            AvailableTime time = availableTimeDao.GetByTimeId(reservationDto.TimeId);
            if (time == null)
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NoAvailableTime()));
            if (!DateTimeUtil.DateMatch(reservationDto.Date))
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.WrongDateformat()));
            int userId = authUtil.GetIdFromToken();
            string dateStr = DateTimeUtil.GetDateStr(reservationDto.Date, time.StartTime);
            int length = (int)redis.ListLength(dateStr);
            if (length % 2 == 0)
            {
                redis.ListRightPush(dateStr, userId);
            }
            else
            {
                int oppUserid = (int)redis.ListGetByIndex(dateStr, length - 2);
                var quizAList = await recommendService.RecommendAsync(userId);
                var quizBList = await recommendService.RecommendAsync(oppUserid);
                User oppUser = userDao.GetUserById(oppUserid);
                User curUser = userDao.GetUserById(userId);
                Quiz quiza = quizAList[0]; Quiz quizb = quizBList[0];
                DateTime reserveDate = DateTime.Parse(reservationDto.Date);
                Reservation tar = new Reservation(time, quiza, quizb, oppUser, curUser, reserveDate);
                reservationDao.AddReservation(tar);
                // 确认预约信息插入成功后再修改redis
                redis.ListRightPush(dateStr, userId);
            }
            return Ok(ResponseUtil.SuccessfulResponse("预约成功"));
        }

        [HttpGet("myreservations")]
        [Authorize]
        public IActionResult GetMyReservations()
        {
            int userId = authUtil.GetIdFromToken();
            var reservationList = reservationDao.GetReservations(userId);
            IList<ReservationDto> res = new List<ReservationDto>();
            foreach (var cur in reservationList)
                res.Add(ReservationDto.ToDto(cur, cur.UserBId == userId));
            return Ok(ResponseUtil.SuccessfulResponse("获取所有预约成功", res));
        }

        [HttpGet("allreservations")]
        public IActionResult GetAllReservations()
        {
            List<AvailableTime> allTime = availableTimeDao.GetAllTime();
            List<ReservationCount> res = new List<ReservationCount>();
            foreach(var curTime in allTime) {
                DateTime baseDate = DateTimeUtil.ConcateDateTime(DateTime.Now, curTime.StartTime);
                for (int i = 0; i < 7; i ++ )
                {
                    DateTime date = baseDate.AddDays(i);
                    string dateStr = date.ToString();
                    if (!redis.KeyExists(dateStr)) res.Add(new ReservationCount(date, 0, curTime.TimeId));
                    else
                    {
                        int cnt = (int)redis.ListLength(dateStr);
                        res.Add(new ReservationCount(date, cnt, curTime.TimeId));
                    }
                }
            }
            res.Sort();
            return Ok(ResponseUtil.SuccessfulResponse("成功获取从当前开始7天内的预约消息", res));
        }


        [HttpPost]
        [Authorize]
        public IActionResult PostReservation([FromBody] PostReservationDto reservationDto)
        {
            AvailableTime time = availableTimeDao.GetByTimeId(reservationDto.TimeId);
            if (time == null)
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NoAvailableTime()));
            if (!DateTimeUtil.DateMatch(reservationDto.Date))
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.WrongDateformat()));
            int userId = authUtil.GetIdFromToken();
            string dateStr = DateTimeUtil.GetDateStr(reservationDto.Date, time.StartTime);
            int length = (int)redis.ListLength(dateStr);
            if (length % 2 == 0)
            {
                redis.ListRightPush(dateStr, userId);
            }
            else
            {
                Quiz quiza = quizDao.GetQuizByIndex(authUtil.GetQuizIndex());
                Quiz quizb = quizDao.GetQuizByIndex(authUtil.GetQuizIndex());
                int oppUserid = (int)redis.ListGetByIndex(dateStr, length - 2);
                User oppUser = userDao.GetUserById(oppUserid);
                User curUser = userDao.GetUserById(userId);
                DateTime reserveDate = DateTime.Parse(reservationDto.Date);
                Reservation tar = new Reservation(time, quiza, quizb, oppUser, curUser, reserveDate);
                reservationDao.AddReservation(tar);
                // 确认预约信息插入成功后再修改redis
                redis.ListRightPush(dateStr, userId);
            }
            return Ok(ResponseUtil.SuccessfulResponse("预约成功"));
        }

        [HttpPost("test")]
        public IActionResult test()
        {
            for (int i = 0;i < 700; i ++ )
            {
                quizDao.GetQuizByIndex(i);
            }
            return Ok();
        }

    }
}
