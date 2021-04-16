using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIE.Entity
{
    [Table("blog")]
    public class Blog
    {
        [Column("blog_id")]
        public int BlogId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("post_time")]
        public DateTime PostTime { get; set; }

        public User user;
    }
}
