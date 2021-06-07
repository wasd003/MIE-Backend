using System;
using System.ComponentModel.DataAnnotations.Schema;
using MIE.Entity.Enum;

namespace MIE.Entity
{
    [Table("quiz")]
    public class Quiz
    {
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("quiz_name")]
        public string QuizName { get; set; }

        [Column("difficulty")]
        public QuizDifficulty Difficulty { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("testcase_1")]
        public string TestCase1 { get; set; }

        [Column("testcase_2")]
        public string TestCase2 { get; set; }

        [Column("testcase_3")]
        public string TestCase3 { get; set; }

    }
}
