using Common.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataAccess.Mappings
{
    public class PlaylistMap : ClassMapping<Playlist>
    {
        public PlaylistMap()
        {
            Id(x => x.PlaylistId, map => map.Generator(Generators.Identity));
            List(x => x.List, map => map.Cascade(Cascade.All), action => action.ManyToMany());
        }
    }
}
