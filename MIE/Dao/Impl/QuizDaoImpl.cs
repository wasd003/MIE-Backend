using System;
using System.Collections.Generic;
using System.Linq;
using MIE.Entity;

namespace MIE.Dao.Impl
{
    public class QuizDaoImpl : IQuizDao
    {
        private readonly MySQLDbContext context;

        public QuizDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public Quiz GetQuizById(int id)
            => context.Quiz.FirstOrDefault(t => t.QuizId == id);

    }
}
