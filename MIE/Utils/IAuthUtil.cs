using System;
using MIE.Entity;

namespace MIE.Utils
{
    public interface IAuthUtil
    {
        public string GetToken(User user);

        public int GetIdFromToken();

        public int GetQuizIndex();

    }
}
