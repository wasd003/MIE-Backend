using System;
using MIE.Entity;

namespace MIE.Dao {
  public interface IUserDao {
    User GetUserById(int id);

    User GetUserByUsername(string username);

    bool AddUser(User user);

  }
}
