using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Entities;

namespace SongMangment.Application
{
    public interface ISongManager
    {
        Song GetSongById(uint songId);
        IEnumerable<Song> GetSongsByQuery(string query);
        Stream GetSongStream(uint songId);
        Task<IEnumerable<string>> SaveSongFiles(IEnumerable<HttpContent> files);
    }
}
