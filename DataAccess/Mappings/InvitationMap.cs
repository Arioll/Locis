using Common.Entities;
using Common.Other;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace DataAccess.Mappings
{
    public class InvitationMap : ClassMapping<Invitation>
    {
        public InvitationMap()
        {
            Property(x => x.CreateDate);
            Property(x => x.RoomId);
            Property(x => x.RoomName);
            Property(x => x.TargetId);
            Property(x => x.SenderName);
            Id(x => x.InvitationId, map => map.Generator(Generators.Identity));
            Property(x => x.Type, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<NotificationTypes>>();
            });
        }
    }
}