using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Journalist;

namespace Common.Entities
{
    public class Password
    {
        public Password(string pass)
        {
            if (Regex.IsMatch(pass, "^.{6,18}$"))
            {
                var md5Hasher = MD5.Create();
                var hash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(pass));
                var builder = new StringBuilder();
                foreach (var a in hash)
                {
                    builder.Append(a.ToString("x2"));
                }
                Value = builder.ToString();
            }
            else
            {
                throw new ArgumentException("Password does not satisfy security requirements");
            }
        }

        protected Password()
        {
        }

        public string Value { get; protected set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            var o = obj as Password;
            if (o == null) return false;
            return o.Value == Value;
        }

        public static Password FromPlainString(string value)
        {
            Require.NotEmpty(value, nameof(value));
            return new Password { Value = value };
        }

        public static bool IsStringCorrectPassword(string passwordToCheck)
        {
            return Regex.IsMatch(passwordToCheck, "^.{6,18}$");
        }
    }
}