using System.Linq;
using Common.Entities;
using DataAccess.Application;
using Journalist;
using PlaylistMangment.Application;

namespace PlaylistMangment.Domain
{
    public class PlaylistManager : IPlaylistManager
    {
        public PlaylistManager(IPlaylistRepository playlistRepository,
            IUserRepository userRepository, 
            IRoomRepository roomRepository, 
            ISongRepository songRepository)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _songRepository = songRepository;
        }

        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISongRepository _songRepository;
        private readonly IRoomRepository _roomRepository;

        public Playlist GetPlaylistById(uint id)
        {
            return _playlistRepository.GetPlaylistById(id);
        }

        public void AddSongsInUsersPlaylist(uint[] songIds, uint userId)
        {
            Require.NotNull(songIds, nameof(songIds));
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            var userPlaylist = _playlistRepository.GetPlaylistById(user.PlaylistId);

            var songs = _songRepository.GetSongsByIds(songIds);
            foreach (var song in songs)
            {
                userPlaylist.Add(song);
            }
            _playlistRepository.UpdatePlaylist(userPlaylist);
        }

        public void UploadUsersPlaylistToCurrentRoom(uint userId)
        {
            Require.Positive(userId, nameof(userId));

            var user = _userRepository.GetUserById(userId);
            var room = _roomRepository.GetRoomById(user.CurrentRoomId);
            var userPlaylist = _playlistRepository.GetPlaylistById(user.PlaylistId);
            var roomPlaylist = _playlistRepository.GetPlaylistById(room.PlaylistId);
            
            roomPlaylist.AddSongsFromAnotherPlaylist(userPlaylist);
            _playlistRepository.UpdatePlaylist(roomPlaylist);
        }

        public void RemoveSongFromPlaylist(uint playlistId, uint songId)
        {
            Require.Positive(playlistId, nameof(playlistId));
            Require.Positive(songId, nameof(songId));

            var playlist = _playlistRepository.GetPlaylistById(playlistId);
            if (playlist.All(song => song.SongId != songId)) return;
            playlist.Remove(playlist.First(song => song.SongId == songId));
            _playlistRepository.UpdatePlaylist(playlist);
        }
    }
}
