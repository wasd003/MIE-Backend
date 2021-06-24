using System;
using MIE.Entity;

namespace MIE.Dto
{
    public class BlogGetDto
    {
        public BlogGetDto(int blogId, int userId, string username, string title, string content, DateTime postTime)
        {
            BlogId = blogId;
            UserId = userId;
            Username = username;
            Title = title;
            Content = content;
            PostTime = postTime;
        }

        public int BlogId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PostTime { get; set; }

        public static BlogGetDto toDto(Blog blog)
        {
            return new BlogGetDto(blog.BlogId, blog.UserId, blog.User.Username, blog.Title,
                blog.Content, blog.PostTime);
        }
    }
}
