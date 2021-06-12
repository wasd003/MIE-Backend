using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Entity;
using MIE.Entity.Enum;
using MIE.Utils;
using StackExchange.Redis;


namespace MIE.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IUserDao userDao;
        private readonly IAuthUtil authUtil;
        private readonly IConnectionMultiplexer redis;

        public AuthController(IUserDao userDao, IAuthUtil authUtil, IConnectionMultiplexer redis)
        {
            this.userDao = userDao;
            this.authUtil = authUtil;
            this.redis = redis;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginUser)
        {
            string username = loginUser.Username;
            string password = loginUser.Password;
            User tar = userDao.GetUserByUsername(username);
            if (tar != null && tar.Password == password)
                return Ok(ResponseUtil.SuccessfulResponse("成功登陆",
                    new { token = authUtil.GetToken(tar) }));
            else
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.WrongUsernameOrPwd()));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user.Password == null || user.Password.Length < 6 || user.Username == null)
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.ShortPwdOrEmptyUsername()));
            if (userDao.GetUserByUsername(user.Username) != null)
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.DuplicateUser()));

            bool addRes = userDao.AddUser(user);
            return addRes ? Ok(ResponseUtil.SuccessfulResponse("注册成功")) :
                      Ok(ResponseUtil.ErrorResponse(ResponseEnum.FailToRegister()));
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestToken()
        {
            IDatabase db = redis.GetDatabase();
            return Ok(new { id = authUtil.GetIdFromToken()});
        }
    }
}
