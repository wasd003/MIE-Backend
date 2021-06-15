using System;
using MIE.Entity;

namespace MIE.Dto
{
    public class SubmissionGetDto
    {
        public SubmissionGetDto(int submissionId, int quizId, int userId, string code, string judgeResult)
        {
            SubmissionId = submissionId;
            QuizId = quizId;
            UserId = userId;
            Code = code;
            JudgeResult = judgeResult;
        }

        public int SubmissionId { get; set; }

        public int QuizId { get; set; }

        public int UserId { get; set; }

        public string Code { get; set; }

        public string JudgeResult { get; set; }

        public static SubmissionGetDto ToDto(Submission submission)
            => new SubmissionGetDto(submission.SubmissionId, submission.QuizId, submission.UserId,
                submission.Code, submission.JudgeResult.ToString());
    }
}
