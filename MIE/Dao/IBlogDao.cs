using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IBlogDao
    {
        bool AddBlog(Blog blog);

        List<Blog> GetBlogListByUserId(int userId);
    }
}
