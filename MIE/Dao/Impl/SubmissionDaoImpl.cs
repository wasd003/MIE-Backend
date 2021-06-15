using System;
using System.Collections.Generic;
using System.Linq;
using MIE.Entity;

namespace MIE.Dao.Impl
{
    public class SubmissionDaoImpl : ISubmissionDao
    {
        private readonly MySQLDbContext context;

        public SubmissionDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public List<Submission> GetSubmissionListByUserIdAndQuizId(int userId, int quizId)
            => context.Submission.Where(t => t.UserId == userId && t.QuizId == quizId).ToList();

        public bool InsertSubmission(Submission tar)
        {
            context.Submission.Add(tar);
            return context.SaveChanges() > 0;
        }
    }
}
