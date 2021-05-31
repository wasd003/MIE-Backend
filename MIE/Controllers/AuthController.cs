using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Entity;
using MIE.Utils;


namespace MIE.Controllers {
  [Route("/auth")]
  public class AuthController : Controller {
    private readonly IUserDao userDao;
    private readonly IAuthUtil authUtil;

    public AuthController(IUserDao userDao, IAuthUtil authUtil) {
      this.userDao = userDao;
      this.authUtil = authUtil;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginUser) {
      string username = loginUser.Username;
      string password = loginUser.Password;
      User tar = userDao.GetUserByUsername(username);
      if (tar != null && tar.Password == password) {
        return Ok(new { token = authUtil.GetToken(tar) });
      } else {
        return Ok(new { msg = "未找到该用户"});
      }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user) {
      if (user.Password == null || user.Password.Length < 6 || user.Username == null)
        return Ok(new { msg = "密码太短或者用户名为空"});
      if (userDao.GetUserByUsername(user.Username) != null)
        return Ok(new { msg = "用户名重复"});

      bool addRes = userDao.AddUser(user);
      return addRes ? Ok(new { msg = "注册成功" }) : Ok(new { msg = "注册失败" });
    }

    [Authorize]
    [HttpGet]
    public IActionResult TestToken() {
      return Ok(new { id = authUtil.GetIdFromToken() });
    }
  }
}
