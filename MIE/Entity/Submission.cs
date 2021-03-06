using System;
using System.ComponentModel.DataAnnotations.Schema;
using MIE.Entity.Enum;

namespace MIE.Entity
{
    [Table("submission")]
    public class Submission
    {
        public Submission(int quizId, int userId, int categoryId, string code, JudgeResultEnum judgeResult)
        {
            QuizId = quizId;
            UserId = userId;
            CategoryId = categoryId;
            Code = code;
            JudgeResult = judgeResult;
        }

        [Column("submission_id")]
        public int SubmissionId { get; set; }

        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("judge_result")]
        public JudgeResultEnum JudgeResult { get; set; }



    }
}
