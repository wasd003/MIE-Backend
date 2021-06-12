using System;
namespace MIE.Entity
{
    public class MatchPair
    {
        public int UserAId { get; set; }

        public string UserAName { get; set; }

        public string UserBId { get; set; }

        public string UserBName { get; set; }

        public MatchPair(int userAId, string userAName, string userBId, string userBName)
        {
            UserAId = userAId;
            UserAName = userAName;
            UserBId = userBId;
            UserBName = userBName;
        }
    }
}
