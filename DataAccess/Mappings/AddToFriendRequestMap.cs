using Common.Entities;
using Common.Other;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace DataAccess.Mappings
{
    public class AddToFriendRequestMap : ClassMapping<AddToFriendRequest>
    {
        public AddToFriendRequestMap()
        {
            Id(x => x.RequestId, map => map.Generator(Generators.Identity));
            Property(x => x.SenderId);
            Property(x => x.TargetId);
            Property(x => x.SenderName);
            Property(x => x.Type, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<NotificationTypes>>();
            });
            Property(x => x.SendDate);
        }
    }
}
