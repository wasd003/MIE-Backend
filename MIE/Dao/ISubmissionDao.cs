using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface ISubmissionDao
    {
        bool InsertSubmission(Submission tar);

        List<Submission> GetSubmissionListByUserIdAndQuizId(int userId, int quizId);
    }
}
