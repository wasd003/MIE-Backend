using System;
using System.Collections.Generic;
using MIE.Dto;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IBlogDao
    {
        bool AddBlog(Blog blog);

        List<Blog> GetBlogListByUserId(int userId);

        List<Blog> GetBlogByPageId(int pageId);

        List<Blog> GetAllBlogs();

        List<BlogGetDto> GetAllBlogDto();

        BlogGetDto GetBlogDtoById(int id);

        BlogGetDto GetBlogDtoBySkip(int skip);

        int GetBlogCount();

    }
}
