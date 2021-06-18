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

        public List<Tuple<int, int>> GetScore(int userId, int[] categoryId)
        {
            List<Submission> submissions = context.Submission.Where(i => i.UserId == userId).ToList();
            var categoryIdSet = new HashSet<int>(categoryId);
            var hash = new Dictionary<int, int>();
            foreach (var x in categoryId) hash[x] = 0;
            foreach (var cur in submissions)
            {
                if (categoryIdSet.Contains(cur.CategoryId))
                {
                    hash[cur.CategoryId]++;
                    if (cur.JudgeResult == Entity.Enum.JudgeResultEnum.AC)
                        hash[cur.CategoryId] += 2;
                    else
                        hash[cur.CategoryId] -= 2;
                }
            }
            var ans = new List<Tuple<int, int>>();
            foreach (var cur in hash)
            {
                ans.Add(Tuple.Create(cur.Value, cur.Key));
            }
            ans.Sort();
            return ans;
        }
    }
}
