using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Entity;
using MIE.Utils;


namespace MIE.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IUserDao userDao;
        private readonly IAuthUtil authUtil;

        public AuthController(IUserDao userDao, IAuthUtil authUtil)
        {
            this.userDao = userDao;
            this.authUtil = authUtil;
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
                return Ok(ResponseUtil.ErrorResponse(10000, "用户名或密码错误"));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user.Password == null || user.Password.Length < 6 || user.Username == null)
                return Ok(ResponseUtil.ErrorResponse(10001, "密码太短或者用户名为空"));
            if (userDao.GetUserByUsername(user.Username) != null)
                return Ok(ResponseUtil.ErrorResponse(10002, "用户已存在"));

            bool addRes = userDao.AddUser(user);
            return addRes ? Ok(ResponseUtil.SuccessfulResponse("注册成功")) :
                      Ok(ResponseUtil.ErrorResponse(10003, "注册失败"));
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestToken()
        {
            return Ok(new { id = authUtil.GetIdFromToken() });
        }
    }
}
