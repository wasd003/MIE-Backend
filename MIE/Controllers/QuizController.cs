using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIE.Dao;
using MIE.Dto;
using MIE.Entity;
using MIE.Entity.Enum;
using MIE.Utils;
using Nest;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MIE.Controllers
{
    [Route("/quiz")]
    public class QuizController : Controller
    {
        private readonly IQuizDao quizDao;
        private readonly IAuthUtil authUtil;
        private readonly ISubmissionDao submissionDao;
        private readonly IDatabase redis;
        private readonly IElasticClient elasticClient;

        public QuizController(IQuizDao quizDao, IAuthUtil authUtil, ISubmissionDao submissionDao,
            IConnectionMultiplexer connectionMultiplexer, IElasticClient elasticClient)
        {
            this.quizDao = quizDao;
            this.authUtil = authUtil;
            this.submissionDao = submissionDao;
            this.redis = connectionMultiplexer.GetDatabase();
            this.elasticClient = elasticClient;
        }

        [HttpGet]
        public IActionResult GetAllQuizs([FromQuery] int pageId)
        {
            if (pageId < 0) return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NegativePageId()));
            var res = quizDao.GetQuizByPageId(pageId);
            return Ok(ResponseUtil.SuccessfulResponse("成功获取数据", res));
        }

        [HttpGet("api/recommend")]
        [Authorize]
        public async Task<IActionResult> RecommendQuizByModelAsync()
        {
            int userId = authUtil.GetIdFromToken();
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
            var pred = await quizDao.PredictByLrAsync(userId, quizList);
            List<Quiz> res = new List<Quiz>();
            for (int i = 0; i < pred.Count; i++)
                if (pred[i].Item1 == true)
                    res.Add(pred[i].Item2);
            return Ok(ResponseUtil.SuccessfulResponse("推荐成功", res));
        }

        [HttpGet("api/search")]
        public IActionResult SearchQuizByElasticSearch([FromQuery] string query)
        {
            var searchResponse = elasticClient.Search<Quiz>(s => s
                .From(0)
                .Size(Constants.MAX_SEARCH_COUNT)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.QuizName)
                        .Query(query)
                     )
                )
            );
            return Ok(ResponseUtil.SuccessfulResponse("搜索成功", searchResponse));
        }

        [HttpGet("concrete")]
        public IActionResult GetConcreteQuiz([FromQuery] int quizId)
        {
            var res = quizDao.GetQuizById(quizId);
            if (res == null) return Ok(ResponseUtil.ErrorResponse(ResponseEnum.QuizNotExist()));
            return Ok(ResponseUtil.SuccessfulResponse("成功获得题目", res));
        }

        [HttpGet("search")]
        public IActionResult SearchQuizzes([FromQuery] string q)
        {
            List<Quiz> quizzes = quizDao.GetAllQuizzes();
            var df = new Dictionary<char, int>();
            var res = new List<Tuple<double, Quiz>>();
            foreach (var ch in q) df[ch] = 0;
            foreach (var quiz in quizzes)
                foreach (var ch in quiz.QuizName)
                    if (df.ContainsKey(ch))
                        df[ch]++;
            foreach (var quiz in quizzes)
            {
                var idf = new Dictionary<char, int>();
                double score = 0.0;
                foreach (var ch in quiz.QuizName)
                {
                    if (!idf.ContainsKey(ch)) idf[ch] = 0;
                    idf[ch]++;
                }
                foreach (var ch in q)
                {
                    if (idf.ContainsKey(ch)) score += (double)idf[ch] / df[ch];
                }
                res.Add(Tuple.Create(score, quiz));
            }
            res = res.OrderByDescending(x => x.Item1).ToList();
            List<Quiz> ans = new List<Quiz>();
            for (int i = 0; i < Constants.MAX_SEARCH_COUNT; i++)
            {
                if (i >= res.Count || res[i].Item1 == 0) break;
                ans.Add(res[i].Item2);
            }
            return Ok(ResponseUtil.SuccessfulResponse("成功搜索", ans));
        }

        [HttpPost("submit")]
        [Authorize]
        public IActionResult OnlineJudge([FromBody] SubmissionPostDto submissionPostDto)
        {
            submissionPostDto.Lang = submissionPostDto.Lang.ToLower();
            if (!Constants.SUPPORT_LANG.Contains(submissionPostDto.Lang))
            {
                return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NotSupportLang()));
            }
            int userId = authUtil.GetIdFromToken();
            Quiz quiz = quizDao.GetQuizById(submissionPostDto.QuizId);
            var body = new
            {
                code = submissionPostDto.Code,
                input = quiz.TestCaseIn,
                expected = quiz.TestCaseOut,
                userId = userId,
                quizId = quiz.QuizId,
                lang = submissionPostDto.Lang
            };
            string jsonBody = JsonConvert.SerializeObject(body);
            string result;
            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                result = client.PostAsync(Constants.JUDGE_MACHINE_URL, content)
                    .Result.Content.ReadAsStringAsync().Result;
            }
            var ans = JsonConvert.DeserializeObject<SubmitResultDto>(result);
            Submission submission = new Submission(submissionPostDto.QuizId, userId, quiz.CategoryId,
                submissionPostDto.Code, Enum.Parse<JudgeResultEnum>(ans.Result));
            submissionDao.InsertSubmission(submission);
            return Ok(ResponseUtil.SuccessfulResponse("判题成功", ans));
        }

        [HttpGet("submit/record")]
        public IActionResult GetSubmitRecords([FromQuery] int quizId)
        {
            int userId = authUtil.GetIdFromToken();
            var submissionList = submissionDao.GetSubmissionListByUserIdAndQuizId(userId, quizId);
            List<SubmissionGetDto> ans = new List<SubmissionGetDto>();
            foreach (var x in submissionList)
                ans.Add(SubmissionGetDto.ToDto(x));
            return Ok(ResponseUtil.SuccessfulResponse("获取提交记录成功", ans));
        }

        [HttpGet("recommend")]
        [Authorize]
        public IActionResult RecommendQuizzes()
        {
            int userId = authUtil.GetIdFromToken();
            string key = "recommend/" + userId;
            if (!redis.KeyExists(key))
            {
                foreach (var cur in Constants.DEFAULT_RECOMMEND)
                {
                    redis.ListRightPush(key, cur);
                }
            }
            int n = (int)redis.ListLength(key);
            int[] categoryIdList = new int[n];
            for (int i = 0; i < n; i++) categoryIdList[i] = (int)redis.ListGetByIndex(key, i);
            var score = submissionDao.GetScore(userId, categoryIdList);
            List<Quiz> res = new List<Quiz>();
            for (int i = 0; i < 5 && i < score.Count; i ++ )
            {
                res.AddRange(quizDao.GetByCategoryId(score[i].Item2, 5 - i));
            }
            return Ok(ResponseUtil.SuccessfulResponse("推荐成功", res));
        }

    }
}
