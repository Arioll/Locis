using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Journalist;

namespace Common.Entities
{
    public class Playlist : IEnumerable<Song>
    {
        public Playlist(IEnumerable<Song> enumerable)
        {
            Require.NotNull(enumerable, nameof(enumerable));

            List = new List<Song>(enumerable);
        }

        public Playlist()
        {
            List = new List<Song>();
        }

        public virtual bool Add(Song element)
        {
            Require.NotNull(element, nameof(element));

            if (List.Any(x => x.SongId == element.SongId)) return false;
            List.Add(element);
            return true;
        }

        public virtual bool Remove(Song element)
        {
            Require.NotNull(element, nameof(element));

            return List.All(song => song.SongId != element.SongId) || 
                List.Remove(List.First(song => song.SongId == element.SongId));
        }

        public virtual void UnionWith(IEnumerable<Song> playlistToFuse)
        {
            Require.NotNull(playlistToFuse, nameof(playlistToFuse));

            foreach (var id in playlistToFuse)
            {
                Add(id);
            }
        }

        public virtual void Clear()
        {
            List = new List<Song>();
        }

        public virtual void AddSongsFromAnotherPlaylist(Playlist playlist)
        {
            Require.NotNull(playlist, nameof(playlist));

            if (List.Count < playlist.List.Count)
            {
                var list = List;
                List = playlist.List;
                playlist.List = list;
            }
            var step = List.Count / playlist.Count;
            var songs = new List<Song>();
            var count = List.Count + playlist.Count - playlist.List.Count(x => List.Contains(x));
            var listEnumerator = List.GetEnumerator();
            var playlistEnumerator = playlist.List.GetEnumerator();
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < step; j++)
                {
                    if (listEnumerator.MoveNext())
                    {
                        songs.Add(listEnumerator.Current);
                    }
                }
                if (playlistEnumerator.MoveNext())
                {
                    songs.Add(playlistEnumerator.Current);
                }
            }
            listEnumerator.Dispose();
            playlistEnumerator.Dispose();
            List = songs;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<Song> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public virtual int Count => List.Count;
        public virtual uint PlaylistId { get; protected set; }
        public virtual IList<Song> List { get; protected set; }
    }
}
