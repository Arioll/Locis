using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using UserMangment.Application;
using System.Collections.Concurrent;
using Journalist;
using UserMangment.Domain.Authorization.Exceptions;

namespace UserMangment.Domain.Authorization
{
    public class Authorization : IAuthorization
    {
        public Authorization(TimeSpan tokenLifeTime, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            TokenLifeTime = tokenLifeTime;
        }

        private readonly IUserRepository _userRepository;
        private readonly ConcurrentDictionary<string, AccessTokenInfo> _tokensWithCreationsTime = 
            new ConcurrentDictionary<string, AccessTokenInfo>();
        public TimeSpan TokenLifeTime { get; }

        public AccessTokenInfo GetTokenInfo(string accessToken)
        {
            Require.NotEmpty(accessToken, nameof(accessToken));
            if (!_tokensWithCreationsTime.ContainsKey(accessToken))
            {
                return null;
            }
            var token = _tokensWithCreationsTime[accessToken];
            if (token.CreateOrUpdateTime + TokenLifeTime < DateTime.Now)
            {
                _tokensWithCreationsTime.TryRemove(token.Token, out token);
                return null;
            }

            token.CreateOrUpdateTime = DateTime.Now;
            return token;
        }

        public bool CancelTokenById(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var token = _tokensWithCreationsTime.Values.FirstOrDefault(value => value.UserId == userId);
            if (token == null) return true;

            AccessTokenInfo info;
            return _tokensWithCreationsTime.TryRemove(token.Token, out info);
        }

        public AccessTokenInfo Autorize(string email, Password password)
        {
            Require.NotEmpty(email, nameof(email));
            Require.NotNull(password, nameof(password));

            var user = _userRepository.GetUsersByPredicate(x => x.Email.Address == email && x.IsDeleted == false).ToList();
            if (user.Count < 1)
            {
                throw new UserNotFoundException("User not found");
            }
            if (user[0].Password.Value != password.Value)
            {
                throw new UnauthorizedAccessException("User is not authorized");
            }

            var token = TakeTokenByUserId(user[0].UserId);
            if (token != null)
            {
                return token;
            }
            var createdToken = GenerateNewToken(user[0].UserId);
            _tokensWithCreationsTime.AddOrUpdate(createdToken.Token, createdToken, (oldToken, info) => createdToken);

            return createdToken;
        }

        private AccessTokenInfo TakeTokenByUserId(uint userId)
        {
            var token = _tokensWithCreationsTime.SingleOrDefault(x => x.Value.UserId == userId);
            if (!token.Equals(default(KeyValuePair<string, AccessTokenInfo>)))
            {
                return token.Value;
            }
            return null;
        }

        private AccessTokenInfo GenerateNewToken(uint userId)
        {
            var guid = Guid.NewGuid();
            var token = guid.ToString().Replace("-", "").ToUpper();
            return new AccessTokenInfo(userId, token, DateTime.Now);
        }
    }
}