using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o;
using Sokoban.DataTypes;
namespace Sokoban.DataAccessLayer
{
    public class UsersManager : DataManager
    {
        public IQueryable<Sokoban.DataTypes.User> GetAllUsers()
        {
            return (from Sokoban.DataTypes.User u in Client select u).AsQueryable<Sokoban.DataTypes.User>();
        }

        public IQueryable<Sokoban.DataTypes.User> GetUserByUsername(string username)
        {
            return (from Sokoban.DataTypes.User u in Client where u.Username == username select u).AsQueryable<Sokoban.DataTypes.User>();
        }

        public void InsertUser(Sokoban.DataTypes.User user)
        {
            user.Id = (from Sokoban.DataTypes.User u in Client select u).Count() + 1;
            Client.Store(user);
            Client.Commit();
        }

        public void UpdateUser(Sokoban.DataTypes.User user)
        {
            Client.Store(user);
            Client.Commit();
        }

        public IQueryable<UserRanking> GetUsersRanking()
        {
            IQueryable<UserRanking> usersRanking = (from Sokoban.DataTypes.User u in Client 
                         select new UserRanking
                         {
                             Username = u.Username  ,
                             Level = (from Progress p in Client where p.UserId== u.Id select p).Count() +1    ,
                             Score = u.Score
                         }).OrderByDescending(u => u.Score).AsQueryable<UserRanking>();

            return usersRanking;
        }
    }
}
