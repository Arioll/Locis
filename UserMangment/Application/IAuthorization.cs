using System;
using Common.Entities;
using UserMangment.Domain.Authorization;

namespace UserMangment.Application
{
    public interface IAuthorization
    {
        TimeSpan TokenLifeTime { get; }
        AccessTokenInfo GetTokenInfo(string accessToken);
        AccessTokenInfo Autorize(string email, Password password);
        bool CancelTokenById(uint userId);
    }
}