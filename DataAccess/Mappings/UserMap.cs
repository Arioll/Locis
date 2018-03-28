using Common.Entities;
using DataAccess.Mappings.Application;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataAccess.Mappings
{
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(x => x.UserId, map => map.Generator(Generators.Identity));
            Property(x => x.Nickname, map => map.NotNullable(true));
            Property(x => x.Password, mapper =>
            {
                mapper.Column("Password");
                mapper.Type<PasswordType>();
                mapper.NotNullable(true);
            });
            Property(x => x.Email, mapper =>
            {
                mapper.Column("Email");
                mapper.Unique(true);
                mapper.Type<MailAddressType>();
                mapper.NotNullable(true);
            });
            Property(x => x.CurrentRoomId);
            Property(x => x.AvatarPicture);
            Property(x => x.IsDeleted);
            Property(x => x.PlaylistId);
            Set(x => x.Rooms, map =>
            {
                map.Cascade(Cascade.All);
                map.Key(c => c.Column("UserId"));
                map.Table("room_user");
            }, action => action.ManyToMany(m => m.Column("RoomId")));
            Set(x => x.Invaitations, map => map.Cascade(Cascade.All), action => action.OneToMany());
            Set(x => x.Friends, map => map.Cascade(Cascade.All), action => action.ManyToMany());
            Set(x => x.AddToFriendRequests, map => map.Cascade(Cascade.All), action => action.OneToMany());
        }
    }
}