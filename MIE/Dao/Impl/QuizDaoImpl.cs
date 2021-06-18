using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ML;
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

        // 获取用户对某题的提交记录数据统计
        public SubmissionDetail GetSubmissionDetail(int userId, int quizId)
        {
            List<Submission> submissions = context.Submission
                .Where(cur => cur.UserId == userId && cur.QuizId == quizId).ToList();
            int submissionCnt = submissions.Count;
            int acCnt = 0, tleCnt = 0, reCnt = 0, ceCnt = 0, waCnt = 0;
            foreach (var i in submissions)
            {
                switch (i.JudgeResult)
                {
                    case Entity.Enum.JudgeResultEnum.WA:
                        waCnt++;
                        break;
                    case Entity.Enum.JudgeResultEnum.AC:
                        acCnt++;
                        break;
                    case Entity.Enum.JudgeResultEnum.TLE:
                        tleCnt++;
                        break;
                    case Entity.Enum.JudgeResultEnum.RE:
                        reCnt++;
                        break;
                    case Entity.Enum.JudgeResultEnum.CE:
                        ceCnt++;
                        break;
                }
            }
            return new SubmissionDetail(submissionCnt, acCnt, waCnt, tleCnt, ceCnt, reCnt);
        }

        public List<Quiz> GetAllQuizzes()
            => context.Quiz.ToList();

        public Quiz GetQuizById(int id)
            => context.Quiz.FirstOrDefault(t => t.QuizId == id);

        public List<Quiz> GetQuizByPageId(int pageId)
            => context.Quiz.OrderBy(x => x.QuizId)
            .Skip(pageId * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE).ToList();

        public List<Quiz> GetByCategoryId(int category, int cnt)
        {
            return context.Quiz.Where(cur => cur.CategoryId == category).Take(cnt).ToList();
        }

        public async Task<List<Tuple<bool, Quiz>>> PredictByLrAsync(int userId, List<Quiz> quizzes)
        {
            MLContext mlContext = new MLContext();
            DataViewSchema modelSchema;
            ITransformer model;
            using (HttpClient client = new HttpClient())
            {
                Stream modelFile = await client.GetStreamAsync(Constants.REMOTE_MODEL_ADDR);
                model = mlContext.Model.Load(modelFile, out modelSchema);
            }
            var predictionFunction = mlContext.Model.CreatePredictionEngine<SubmissionDetail, QuizPrediction>(model);
            List<Tuple<bool, Quiz>> res = new List<Tuple<bool, Quiz>>();
            foreach (var quiz in quizzes)
            {
                var submissionDetail = GetSubmissionDetail(userId, quiz.QuizId);
                var resultPrediction = predictionFunction.Predict(submissionDetail);
                res.Add(Tuple.Create(resultPrediction.Prediction, quiz));
            }
            return res;
        }

        
    }
}
