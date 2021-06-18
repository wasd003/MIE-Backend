using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IQuizDao
    {
        Quiz GetQuizById(int id);

        List<Quiz> GetQuizByPageId(int pageId);

        List<Quiz> GetAllQuizzes();

        List<Quiz> GetByCategoryId(int category, int cnt);

        Task<List<Tuple<bool, Quiz>>> PredictByLrAsync(int userId, List<Quiz> quizzes);
    }
}
