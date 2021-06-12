using System;
using System.ComponentModel.DataAnnotations.Schema;
using MIE.Entity.Enum;

namespace MIE.Entity
{
    [Table("luogu_quiz")]
    public class Quiz
    {
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("quiz_name")]
        public string QuizName { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("testcase_in")]
        public string TestCaseIn { get; set; }

        [Column("testcase_out")]
        public string TestCaseOut { get; set; }

        [Column("difficulty")]
        public string Difficulty { get; set; }

        [Column("algorithm")]
        public string Algorithm { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

    }
}
