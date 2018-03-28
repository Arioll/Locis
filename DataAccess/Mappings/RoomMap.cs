using Common.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Cascade = NHibernate.Mapping.ByCode.Cascade;

namespace DataAccess.Mappings
{
    public class RoomMap : ClassMapping<Room>
    {
        public RoomMap()
        {
            Table("Rooms");
            Id(x => x.RoomId, map =>
            {
                map.Generator(Generators.HighLow, gmap => gmap.Params(
                    new
                    {
                        max_lo = 1,
                        table = "HiloRooms",
                        column = "NextHi"
                    }));
            });
            Property(x => x.AvatarUri);
            Property(x => x.RoomName);
            Property(x => x.AdministratorId);
            Property(x => x.CurrentSongId);
            Property(x => x.CurrentSongStartTime);
            Property(x => x.PlaylistId);
            Set(x => x.UsersInRoom, mapper =>
            {
                mapper.Cascade(Cascade.All);
                mapper.Key(c => c.Column("RoomId"));
                mapper.Table("room_user");
                mapper.Inverse(true);
            }, action => action.ManyToMany(m => m.Column("UserId")));
        }
    }
}