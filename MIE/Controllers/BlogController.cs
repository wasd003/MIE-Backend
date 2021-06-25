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

        [HttpGet("my")]
        [Authorize]
        public IActionResult GetMyBlog()
        {
            int userId = authUtil.GetIdFromToken();
            var res = blogDao.GetBlogListByUserId(userId);
            var markdown = new MarkdownSharp.Markdown();
            List<BlogGetDto> ans = new List<BlogGetDto>();
            for (int i = 0; i < res.Count; i ++ )
            {
                res[i].Content = markdown.Transform(res[i].Content);
                ans.Add(BlogGetDto.toDto(res[i]));
            }
            return Ok(ResponseUtil.SuccessfulResponse("获取我的博客成功", ans));
        }

        [HttpGet("search")]
        public IActionResult SearchBlog([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.EmptyQ()));
            List<Blog> blogs = blogDao.GetAllBlogs();
            var df = new Dictionary<char, int>();
            var res = new List<Tuple<double, Blog>>();
            foreach (var ch in q) df[ch] = 0;
            foreach (var blog in blogs)
                foreach (var ch in blog.Title)
                    if (df.ContainsKey(ch))
                        df[ch]++;
            foreach (var blog in blogs)
            {
                var idf = new Dictionary<char, int>();
                double score = 0.0;
                foreach (var ch in blog.Title)
                {
                    if (!idf.ContainsKey(ch)) idf[ch] = 0;
                    idf[ch]++;
                }
                foreach (var ch in q)
                {
                    if (idf.ContainsKey(ch)) score += (double)idf[ch] / df[ch];
                }
                res.Add(Tuple.Create(score, blog));
            }
            res = res.OrderByDescending(x => x.Item1).ToList();
            List<Blog> ans = new List<Blog>();
            var markdown = new MarkdownSharp.Markdown();
            for (int i = 0; i < Constants.MAX_SEARCH_COUNT; i++)
            {
                if (i >= res.Count || res[i].Item1 == 0) break;
                res[i].Item2.Content = markdown.Transform(res[i].Item2.Content);
                ans.Add(res[i].Item2);
            }
            return Ok(ResponseUtil.SuccessfulResponse("成功搜索", ans));
        }
    }
}
