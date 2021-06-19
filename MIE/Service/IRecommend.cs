using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MIE.Entity;

namespace MIE.Service
{
    public interface IRecommend
    {
        Task<List<Quiz>> RecommendAsync(int userid);
    }
}
