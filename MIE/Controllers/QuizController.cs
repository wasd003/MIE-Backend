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
using Newtonsoft.Json;

namespace MIE.Controllers
{
    [Route("/quiz")]
    public class QuizController : Controller
    {
        private readonly IQuizDao quizDao;
        private readonly IAuthUtil authUtil;
        private readonly ISubmissionDao submissionDao;

        public QuizController(IQuizDao quizDao, IAuthUtil authUtil, ISubmissionDao submissionDao)
        {
            this.quizDao = quizDao;
            this.authUtil = authUtil;
            this.submissionDao = submissionDao;
        }

        [HttpGet]
        public IActionResult GetAllQuizs([FromQuery] int pageId)
        {
            if (pageId < 0) return Ok(ResponseUtil.ErrorResponse(ResponseEnum.NegativePageId()));
            var res = quizDao.GetQuizByPageId(pageId);
            return Ok(ResponseUtil.SuccessfulResponse("成功获取数据", res));
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
            for (int i = 0; i < Constants.MAX_SEARCH_COUNT; i ++ )
            {
                if (i >= res.Count || res[i].Item1 == 0) break;
                ans.Add(res[i].Item2);
            }
            return Ok(ResponseUtil.SuccessfulResponse("成功搜索", ans));
        }

        [HttpPost("submit")]
        [Authorize]
        public IActionResult OnlineJudge([FromBody] Submission submission)
        {
            submission.UserId = authUtil.GetIdFromToken();
            Quiz quiz = quizDao.GetQuizById(submission.QuizId);
            var body = new
            {
                code = submission.Code,
                input = quiz.TestCaseIn,
                expected = quiz.TestCaseOut,
                userId = submission.UserId,
                quizId = quiz.QuizId
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
            submission.JudgeResult = Enum.Parse<JudgeResultEnum>(ans.Result);
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
        
    }
}
