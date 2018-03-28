using Common.Entities;
using Common.Other;
using NHibernate.Type;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataAccess.Mappings
{
    public class EmailMessageMap : ClassMapping<EmailMessage>
    {
        public EmailMessageMap()
        {
            Id(x => x.MailId, map => map.Generator(Generators.Identity));
            Set(x => x.TargetEmails, map =>
            {
                map.Cascade(Cascade.All);
                map.Lazy(CollectionLazy.NoLazy);
            });
            
            Property(x => x.Link, map => map.NotNullable(true));
            Property(x => x.TargetNickname, map => map.NotNullable(true));
            Property(x => x.OperationType, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<EmailTypes>>();
            });
        }
    }
}
