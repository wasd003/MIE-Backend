using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MIE.Dto;
using MIE.Entity;
using MIE.Utils;

namespace MIE.Dao.Impl
{
    public class BlogDaoImpl : IBlogDao
    {
        private readonly MySQLDbContext context;

        public BlogDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public bool AddBlog(Blog blog)
        {
            context.Add(blog);
            return context.SaveChanges() > 0;
        }

        public List<Blog> GetBlogByPageId(int pageId)
        {
            return context.Blog.Skip(pageId).Take(Constants.PAGE_SIZE).Include(cur => cur.User).
                ToList();
        }

        public List<Blog> GetBlogListByUserId(int userId)
        {
            return context.Blog.Where(cur => cur.UserId == userId)
                .Include(cur => cur.User).ToList();
        }


        public List<Blog> GetAllBlogs() => context.Blog.ToList();

        public BlogGetDto GetBlogDtoById(int id)
        {
            var blog = context.Blog.FirstOrDefault(cur => cur.BlogId == id);
            if (blog == null) return null;
            return ConvertToDto(blog);
        }

        public List<BlogGetDto> GetAllBlogDto()
        {
            List<Blog> blogs = context.Blog.ToList();
            List<BlogGetDto> ans = new List<BlogGetDto>();
            for (int i = 0; i < blogs.Count; i ++ )
            {
                ans.Add(ConvertToDto(blogs[i]));
            }
            return ans;
        }

        public BlogGetDto ConvertToDto(Blog blog)
        {
            if (blog == null) return null;
            var markdown = new MarkdownSharp.Markdown();
            blog.Content = markdown.Transform(blog.Content);
            blog.User = context.User.FirstOrDefault(cur => cur.UserId == blog.UserId);
            return BlogGetDto.toDto(blog);
        }

        public int GetBlogCount()
        {
            return context.Blog.Count();
        }

        public BlogGetDto GetBlogDtoBySkip(int skip)
        {
            var list = context.Blog.Skip(skip).Take(1).ToList();
            if (list == null || list.Count == 0) return null;
            return ConvertToDto(list[0]);
        }
    }
}
