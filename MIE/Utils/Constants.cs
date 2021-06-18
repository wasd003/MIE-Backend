using System;
namespace MIE.Utils
{
    public class Constants
    {
        /**
         * 每页包含的题目数量
         */
        public static int PAGE_SIZE = 20;

        /**
         * 搜索最多返回条数
         */
        public static int MAX_SEARCH_COUNT = 20;

        public static string JUDGE_MACHINE_URL = "http://localhost:7777/submit";

        public static int[] DEFAULT_RECOMMEND = { 237, 242, 268, 269, 273, 121, 96, 101, 121, 128 };

        public static int CANDIDATES_COUNT = 5;

        public static string REMOTE_MODEL_ADDR = "www.wasd003.cn/home/mie/models/";
    }
}
