using System;
using System.Collections.Generic;
using MIE.Entity;

namespace MIE.Dao
{
    public interface IUserDao
    {
        User GetUserById(int id);

        User GetUserByUsername(string username);

        bool AddUser(User user);

        List<User> SearchUserByContainStrategy(string q);

    }
}
