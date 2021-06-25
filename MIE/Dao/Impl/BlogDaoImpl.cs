using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
    }
}
