using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Entity;
using MIE.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MIE.Controllers {
  [Route("/blog")]
  public class BlogController : Controller {
    private readonly IBlogDao blogDao;
    private readonly IAuthUtil authUtil;

    public BlogController(IBlogDao blogDao, IAuthUtil authUtil) {
      this.blogDao = blogDao;
      this.authUtil = authUtil;
    }

    [HttpGet]
    public IActionResult Get() {
      int userId = authUtil.GetIdFromToken();
      var blogList = blogDao.GetBlogListByUserId(userId);
      return Ok(blogList);
    }

    [HttpPost]
    [Authorize]
    public IActionResult PostBlog([FromBody] Blog blog) {
      int userId = authUtil.GetIdFromToken();
      blog.UserId = userId;
      blog.PostTime = DateTime.Now;
      blogDao.AddBlog(blog);
      return Ok("post succesfully");
    }
  }
}
