using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Dto;
using MIE.Entity;
using MIE.Entity.Enum;
using MIE.Utils;


namespace MIE.Controllers
{
    [Route("/blog")]
    public class BlogController : Controller
    {
        private readonly IBlogDao blogDao;
        private readonly IAuthUtil authUtil;

        public BlogController(IBlogDao blogDao, IAuthUtil authUtil)
        {
            this.blogDao = blogDao;
            this.authUtil = authUtil;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] int pageId)
        {
            List <Blog> blogs = blogDao.GetBlogByPageId(pageId);
            var markdown = new MarkdownSharp.Markdown();
            List<BlogGetDto> res = new List<BlogGetDto>();
            for (int i = 0; i < blogs.Count; i ++ )
            {
                blogs[i].Content = markdown.Transform(blogs[i].Content);
                res.Add(BlogGetDto.toDto(blogs[i]));
            }
            return Ok(ResponseUtil.SuccessfulResponse($"成功获取第{pageId}页博客", res));
        }

        [HttpPost]
        [Authorize]
        public IActionResult PostBlog([FromBody] Blog blog)
        {
            if (string.IsNullOrEmpty(blog.Title))
            {
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NoTitle()));
            }
            int userId = authUtil.GetIdFromToken();
            blog.UserId = userId;
            blog.PostTime = DateTime.Now;
            blogDao.AddBlog(blog);
            return Ok(ResponseUtil.SuccessfulResponse("添加博客成功", blog));
        }
    }
}
