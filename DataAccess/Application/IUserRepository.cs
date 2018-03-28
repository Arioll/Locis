using System;
using System.Collections.Generic;
using Common.Entities;

namespace DataAccess.Application
{
    public interface IUserRepository
    {
        User GetUserById(uint userId);
        IEnumerable<User> GetUsersByPredicate(Func<User, bool> predicate);
        uint CreateUser(User userToCreate);
        void UpdateUser(User userToUpdate);
        IEnumerable<User> GetUserByName(string userName);
        IEnumerable<User> GetUsersByIds(IEnumerable<uint> ids);
    }
}