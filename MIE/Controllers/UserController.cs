using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIE.Dao;
using MIE.Entity;

namespace MIE.Controllers {
  [Route("/user")]
  public class UserController : Controller {
    private readonly IUserDao userDao;
    private readonly MySQLDbContext context;

    public UserController(IUserDao userDao, MySQLDbContext context) {
      this.userDao = userDao;
      this.context = context;
    }

    [HttpGet]
    public IActionResult Get() {
      return Ok(userDao.GetUserById(1));
    }
    [HttpPost]
    public IActionResult PostBlog([FromBody] Blog blog) {

      return Ok(context.User.Where(cur => cur.UserId == 1).Include(u => u.BlogList).ToList());
    }

  }
}
