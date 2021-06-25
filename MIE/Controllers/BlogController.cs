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
        private readonly IUserDao userDao;

        public BlogController(IBlogDao blogDao, IAuthUtil authUtil, IUserDao userDao)
        {
            this.blogDao = blogDao;
            this.authUtil = authUtil;
            this.userDao = userDao;
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
            List<BlogGetDto> ans = new List<BlogGetDto>();
            var markdown = new MarkdownSharp.Markdown();
            var h = new Dictionary<int, User>();
            for (int i = 0; i < Constants.MAX_SEARCH_COUNT; i++)
            {
                if (i >= res.Count || res[i].Item1 == 0) break;
                res[i].Item2.Content = markdown.Transform(res[i].Item2.Content);
                int id = res[i].Item2.UserId;
                if (!h.ContainsKey(id)) h[id] = userDao.GetUserById(id);
                res[i].Item2.User = h[id];
                ans.Add(BlogGetDto.toDto(res[i].Item2));
            }
            return Ok(ResponseUtil.SuccessfulResponse("成功搜索", ans));
        }

        [HttpGet("random")]
        public IActionResult GetRandomBlogs()
        {
            List<BlogGetDto> ans = new List<BlogGetDto>();
            Random r = new Random();
            int total = blogDao.GetBlogCount();
            if (total < Constants.RANDOM_COUNTS)
                return Ok(ResponseUtil.SuccessfulResponse(
                    $"数量不足{Constants.RANDOM_COUNTS}", blogDao.GetAllBlogDto()));
            int diff = total / Constants.RANDOM_COUNTS;
            for (int i = 0; i < total && ans.Count < Constants.RANDOM_COUNTS; i += diff)
            {
                int skip = r.Next(i, Math.Min(total, i + diff));
                var cur = blogDao.GetBlogDtoBySkip(skip);
                if (cur != null) ans.Add(cur);
            }
            return Ok(ResponseUtil.SuccessfulResponse("成功获得随机博客", ans));
        }
    }
}
