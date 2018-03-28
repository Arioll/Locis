using System;
using Journalist;

namespace UserMangment.Domain.Authorization
{
    public class AccessTokenInfo
    {
        public AccessTokenInfo(uint userId, string token, DateTime createOrUpdateTime)
        {
            Require.Positive(userId, nameof(userId));
            Require.NotEmpty(token, nameof(token));

            UserId = userId;
            Token = token;
            CreateOrUpdateTime = createOrUpdateTime;
        }

        public string Token { get; private set; }
        public DateTime CreateOrUpdateTime { get; set; }
        public uint UserId { get; private set; }
    }
}
