using Common.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace DataAccess.Mappings
{
    class SongMap : ClassMapping<Song>
    {
        public SongMap()
        {
            Table("Songs");
            Id(x => x.SongId, map =>
            {
                map.Generator(Generators.HighLow, gmap => gmap.Params(
                    new
                    {
                        max_lo = 50,
                        table = "HiloSongs",
                        column = "NextHi"
                    }));
            });
            Property(x => x.Length);
            Property(x => x.Title, map => map.Column("Title"));
            Bag(x => x.Performers, map => map.Cascade(Cascade.All));
            Bag(x => x.Genres, map => map.Cascade(Cascade.All));
            Property(x => x.Album);
            Property(x => x.Path);
            Property(x => x.Year);
            Property(x => x.FileName);
        }
    }
}
