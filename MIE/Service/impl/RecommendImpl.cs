using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIE.Dao;
using MIE.Entity;
using MIE.Utils;
using StackExchange.Redis;

namespace MIE.Service.impl
{
    public class RecommendImpl : IRecommend
    {
        private readonly IDatabase redis;
        private readonly IQuizDao quizDao;

        public RecommendImpl(IConnectionMultiplexer connectionMultiplexer, IQuizDao quizDao)
        {
            redis = connectionMultiplexer.GetDatabase();
            this.quizDao = quizDao;
        }

        public List<Quiz> Recommend(int userId)
        {
            string key = "recommend/" + userId;
            if (!redis.KeyExists(key))  // new suer, cold start, use default category
            {
                foreach (var cur in Constants.DEFAULT_RECOMMEND)
                {
                    redis.ListRightPush(key, cur);
                }
            }
            int n = (int)redis.ListLength(key);
            List<Quiz> quizList = new List<Quiz>();
            for (int i = 0; i < n; i ++ )  // get candidate categoryId
            {
                int categoryId = (int)redis.ListGetByIndex(key, i);
                quizList.AddRange(quizDao.GetByCategoryId(categoryId, Constants.CANDIDATES_COUNT));
            }
            var pred = quizDao.PredictByLr(userId, quizList);
            List<Quiz> res = new List<Quiz>();
            for (int i = 0; i < pred.Count; i++)
                if (pred[i].Item1 == true)
                    res.Add(pred[i].Item2);
            return res;
        }
    }
}
