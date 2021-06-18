using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIE.Entity
{
    [Table("user")]
    public class User
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("group")]
        public string Group { get; set; }

        [Column("position")]
        public string Position { get; set; }

        public List<Blog> BlogList { get; set; }
    }
}
