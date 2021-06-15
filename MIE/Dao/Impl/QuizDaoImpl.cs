using System;
using System.Collections.Generic;
using System.Linq;
using MIE.Entity;
using MIE.Utils;

namespace MIE.Dao.Impl
{
    public class QuizDaoImpl : IQuizDao
    {
        private readonly MySQLDbContext context;

        public QuizDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public List<Quiz> GetAllQuizzes()
            => context.Quiz.ToList();

        public Quiz GetQuizById(int id)
            => context.Quiz.FirstOrDefault(t => t.QuizId == id);

        public List<Quiz> GetQuizByPageId(int pageId)
            => context.Quiz.OrderBy(x => x.QuizId)
            .Skip(pageId * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE).ToList();

    }
}
