using System;
using System.Data;
using System.Data.Common;
using System.Net.Mail;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace DataAccess.Mappings.Application
{
    public class MailAddressType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.GetType() == y.GetType();
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor implementor, object owner)
        {
            var property0 = NHibernateUtil.String.NullSafeGet(rs, names[0], implementor, owner);

            if (property0 == null)
            {
                return null;
            }

            var mailAddress = new MailAddress(property0.ToString());

            return mailAddress;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor implementor)
        {
            if (value == null)
            {
                (cmd.Parameters[index] as IDataParameter).Value = DBNull.Value;
            }
            else
            {
                var state = (MailAddress)value;
                (cmd.Parameters[index] as IDataParameter).Value = state.Address;
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.String.SqlType }; }
        }

        public Type ReturnedType
        {
            get { return typeof(MailAddress); }
        }

        public bool IsMutable
        {
            get { return false; }
        }
    }
}
