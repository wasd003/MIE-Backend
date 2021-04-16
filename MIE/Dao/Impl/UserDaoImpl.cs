using System;
using System.Linq;
using MIE.Entity;

namespace MIE.Dao.Impl
{
    public class UserDaoImpl : IUserDao
    {
        private readonly MySQLDbContext context;


        public UserDaoImpl(MySQLDbContext context)
        {
            this.context = context;
        }

        public bool AddUser(User user)
        {
            context.Add(user);
            return context.SaveChanges() > 0;
        }

        public User GetUserById(int id)
        {
            return context.User.FirstOrDefault(x => x.UserId == id);
        }

        public User GetUserByUsername(string username)
        {
            return context.User.FirstOrDefault(x => x.Username == username);
        }
    }
}
