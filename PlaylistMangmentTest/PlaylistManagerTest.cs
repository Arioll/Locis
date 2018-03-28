using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PlaylistMangment.Domain;
using SongMangment.Domain;

namespace PlaylistMangmentTest
{
    [TestClass]
    public class PlaylistManagerTest
    {
        [TestInitialize]
        public void Setup()
        { 
            _roomRepo = new Mock<IRoomRepository>();
            _userRepo = new Mock<IUserRepository>();
            _songRepo = new Mock<ISongRepository>();
            _playlistRepo = new Mock<IPlaylistRepository>();
        }

        private Mock<IRoomRepository> _roomRepo;
        private Mock<ISongRepository> _songRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IPlaylistRepository> _playlistRepo;

        [TestMethod]
        public void AddSongsInUserPlaylist_SongsWasAdded()
        {
            var generator = new Random();
            var firstSongId = (uint) generator.Next(1, int.MaxValue);
            var secoundSongId = (uint) generator.Next(1, int.MaxValue);
            var userId = (uint) generator.Next(1, int.MaxValue);
            var userPlaylistId = generator.Next(1, int.MaxValue);
            var firstSong = Mock.Of<Song>(x => x.SongId == firstSongId);
            var secoundSong = Mock.Of<Song>(x => x.SongId == secoundSongId);
            var userPlaylist = new Playlist();
            var user = Mock.Of<User>(x =>
                x.UserId == userId &&
                x.PlaylistId == userPlaylistId);
            _playlistRepo.Setup(repo => repo.GetPlaylistById(It.Is<uint>(id => id == userPlaylistId)))
                .Returns(userPlaylist);
            _userRepo.Setup(repo => repo.GetUserById(It.IsAny<uint>())).Returns(user);
            _songRepo.Setup(repo => repo.GetSongsByIds(It.Is<IEnumerable<uint>>(a => a.Contains(firstSongId) && a.Contains(secoundSongId))))
                .Returns(new List<Song> {firstSong, secoundSong});
            var playlistManager = new PlaylistManager(_playlistRepo.Object,
                _userRepo.Object,
                _roomRepo.Object,
                _songRepo.Object);

            playlistManager.AddSongsInUsersPlaylist(new[] { firstSongId, secoundSongId }, userId);

            Assert.IsTrue(userPlaylist.List.Any(x => x.SongId == firstSongId) && userPlaylist.List.Any(x => x.SongId == secoundSongId));
            Assert.IsTrue(userPlaylist.Count == 2);
            _playlistRepo.Verify(repo => repo.UpdatePlaylist(It.IsAny<Playlist>()));
        }

        [TestMethod]
        public void UploadUsersPlaylistToCurrentRoom_PlaylistWasUploaded()
        {
            var generator = new Random();
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var userId = (uint) generator.Next(1, int.MaxValue);
            var userPlaylistId = (uint) generator.Next(1, int.MaxValue);
            var roomPlaylistId = (uint) generator.Next(1, int.MaxValue); 
            var userPlaylistSize = generator.Next(1, 10);
            var roomPlaylistSize = generator.Next(1, 10);
            var userPlaylist = new Playlist();
            var roomPlaylist = new Playlist();
            
            for (int i = 0; i < userPlaylistSize; i++)
            {
                var id = generator.Next(1, int.MaxValue);
                var song = Mock.Of<Song>(x => x.SongId == id);
                if (!userPlaylist.Add(song))
                {
                    i--;
                }
            }
            for (int i = 0; i < roomPlaylistSize; i++)
            {
                var id = generator.Next(1, int.MaxValue);
                var song = Mock.Of<Song>(x => x.SongId == id);
                if (!roomPlaylist.Add(song))
                {
                    i--;
                }
            }
            var userPlaylistCopy = new Playlist(userPlaylist.List);
            var roomPlaylistCopy = new Playlist(roomPlaylist.List);
            foreach (var song in roomPlaylistCopy.List)
            {
                userPlaylistCopy.Add(song);
            }
            var size = userPlaylistCopy.Count;
            var user = Mock.Of<User>(x =>
                x.UserId == userId &&
                x.PlaylistId == userPlaylistId &&
                x.CurrentRoomId == roomId);
            var room = Mock.Of<Room>(x =>
                x.RoomId == roomId &&
                x.PlaylistId == roomPlaylistId);
            _roomRepo.Setup(repo => repo.GetRoomById(It.IsAny<uint>())).Returns(room);
            _userRepo.Setup(repo => repo.GetUserById(It.IsAny<uint>())).Returns(user);
            _playlistRepo.Setup(repo => repo.GetPlaylistById(It.Is<uint>(id => id == userPlaylistId))).Returns(userPlaylist);
            _playlistRepo.Setup(repo => repo.GetPlaylistById(It.Is<uint>(id => id == roomPlaylistId))).Returns(roomPlaylist);
            var playlistManager = new PlaylistManager(_playlistRepo.Object,
                _userRepo.Object,
                _roomRepo.Object,
                _songRepo.Object);

            playlistManager.UploadUsersPlaylistToCurrentRoom(userId);

            Assert.IsTrue(roomPlaylist.Count == size);
            _playlistRepo.Verify(repo => repo.UpdatePlaylist(It.IsAny<Playlist>()));
        }

        [TestMethod]
        public void DeleteSongFromPlaylistTest()
        {
            var random = new Random();
            var playlistId = (uint)random.Next(1, int.MaxValue);
            var songId = (uint)random.Next(1, int.MaxValue);
            var song = Mock.Of<Song>(s => s.SongId == songId);
            var playlist = Mock.Of<Playlist>(pl => pl.PlaylistId == playlistId && pl.List == new List<Song> { song });
            _playlistRepo.Setup(x => x.GetPlaylistById(It.Is<uint>(id => id == playlistId))).Returns(playlist);
            _songRepo.Setup(x => x.GetSongById(It.Is<uint>(id => id == songId))).Returns(song);
            var playlistManager = new PlaylistManager(_playlistRepo.Object,
                _userRepo.Object,
                _roomRepo.Object,
                _songRepo.Object);

            playlistManager.RemoveSongFromPlaylist(playlistId, songId);
            
            Assert.IsTrue(playlist.Count == 0);
            _playlistRepo.Verify(x => x.UpdatePlaylist(playlist));
        }
    }
}
