using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface ISubmissionDao
    {
        bool InsertSubmission(Submission tar);

        List<Submission> GetSubmissionListByUserIdAndQuizId(int userId, int quizId);

        /**
         * [score, category_id]
         */
        List<Tuple<int, int>> GetScore(int userId, int[] categoryId);


    }
}
