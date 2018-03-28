using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(SessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        private readonly SessionProvider _sessionProvider;

        public uint CreateUser(User userToCreate)
        {
            Require.NotNull(userToCreate, nameof(userToCreate));

            var session = _sessionProvider.GetCurrentSession();

            return (uint) session.Save(userToCreate);
        }

        public User GetUserById(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var session = _sessionProvider.GetCurrentSession();

            var user = session.Get<User>(userId);
            return user.IsDeleted ? null : user;
        }

        public IEnumerable<User> GetUserByName(string userName)
        {
            Require.NotEmpty(userName, nameof(userName));

            var session = _sessionProvider.GetCurrentSession();

            return session.Query<User>().Where(profile => profile.Nickname.ToLower().IndexOf(userName.ToLower()) == 0 && !profile.IsDeleted).ToList();
        }

        public IEnumerable<User> GetUsersByPredicate(Func<User, bool> predicate)
        {
            var session = _sessionProvider.GetCurrentSession();

            return predicate == null
                ? session.Query<User>().Where(user => !user.IsDeleted).ToList()
                : session.Query<User>().Where(predicate).Where(user => !user.IsDeleted).ToList();
        }

        public void UpdateUser(User userToUpdate)
        {
            Require.NotNull(userToUpdate, nameof(userToUpdate));

            var session = _sessionProvider.GetCurrentSession();

            session.Update(userToUpdate);
        }

        public IEnumerable<User> GetUsersByIds(IEnumerable<uint> ids)//todo: null checking for got users
        {
            Require.NotNull(ids, nameof(ids));

            var session = _sessionProvider.GetCurrentSession();

            return ids.Select(id => session.Get<User>(id)).ToList();
        }

        public void DeleteUser(User user)
        {
            Require.NotNull(user, nameof(user));

            var session = _sessionProvider.GetCurrentSession();

            session.Delete(user);
        }
    }
}
