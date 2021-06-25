using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIE.Dao;
using MIE.Entity;
using MIE.Entity.Enum;
using MIE.Utils;

namespace MIE.Controllers
{
    [Route("/user")]
    public class UserController : Controller
    {
        private readonly IUserDao userDao;
        private readonly IAuthUtil authUtil;

        public UserController(IUserDao userDao, IAuthUtil auth)
        {
            this.userDao = userDao;
            this.authUtil = auth;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            int userId = authUtil.GetIdFromToken();
            User user = userDao.GetUserById(userId);
            if (user == null) return Ok(ResponseUtil.ErrorResponse(ResponseEnum.UserNotFound()));
            return Ok(ResponseUtil.SuccessfulResponse("成功获得个人信息", user));
        }

        [HttpGet("search")]
        public IActionResult SearchUserByContainStrategy([FromQuery] string q)
        {
            var res = userDao.SearchUserByContainStrategy(q);
            return Ok(ResponseUtil.SuccessfulResponse("搜索成功", res));
        }

    }
}
