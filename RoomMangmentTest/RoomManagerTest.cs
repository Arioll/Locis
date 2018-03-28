using System;
using System.Collections.Generic;
using Common.Entities;
using DataAccess.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RoomMangment.Domain;

namespace RoomMangmentTest
{
    [TestClass]
    public class RoomManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            _roomRepo = new Mock<IRoomRepository>();
            _userRepo = new Mock<IUserRepository>();
            _playlistRepo = new Mock<IPlaylistRepository>();
        }

        private Mock<IRoomRepository> _roomRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IPlaylistRepository> _playlistRepo;

        [TestMethod]
        public void CreateRoomTest_MethodCreateRoomInRoomRepositoryWasCalled()
        {
            _roomRepo.Setup(repository => repository.CreateRoom(It.IsAny<Room>())).Returns(1);
            var user = Mock.Of<User>(x => x.UserId == 1 && x.Rooms == new HashSet<Room>() && x.CurrentRoomId == 0);
            _userRepo.Setup(repository => repository.GetUserById(It.IsAny<uint>())).Returns(user);
            var roomManager = new RoomManager(_roomRepo.Object, 
                _userRepo.Object, 
                _playlistRepo.Object);

            roomManager.CreateRoom("swqswqs", 1);

            Assert.IsTrue(user.CurrentRoomId == 1);
            Assert.IsTrue(user.Rooms.Count == 1);
            _roomRepo.Verify(x => x.CreateRoom(It.IsAny<Room>()));
        }

        [TestMethod]
        public void AddUserInRoom_UserWasUpdateWithNewCurrentRoomId()
        {
            var generator = new Random();
            var userId = generator.Next(1, int.MaxValue);
            var roomId = generator.Next(1, int.MaxValue);
            var room = Mock.Of<Room>(x => x.RoomId == roomId && x.UsersInRoom == new HashSet<User>());
            _roomRepo.Setup(repository => repository.GetRoomById(It.IsAny<uint>())).Returns(room);
            var user = Mock.Of<User>(x => x.CurrentRoomId == 0 && x.UserId == userId && x.Rooms == new HashSet<Room>()); 
            _userRepo.Setup(repositpry => repositpry.GetUserById(It.IsAny<uint>())).Returns(user);
            var roomManager = new RoomManager(_roomRepo.Object,
                _userRepo.Object,
                _playlistRepo.Object);

            roomManager.AddUserInRoom(user.UserId, room.RoomId);

            Assert.IsTrue(user.CurrentRoomId == roomId);
            Assert.IsTrue(room.UsersInRoom.Count == 1);
            Assert.IsTrue(user.Rooms.Count == 1);
        }

        [TestMethod]
        public void DeleteUserFromRoom_UserWasDeletedFromRoom()
        {
            var generator = new Random();
            var firstUserId = (uint) generator.Next(1, int.MaxValue);
            var secoundUserId = (uint) generator.Next(1, int.MaxValue);
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var room = Mock.Of<Room>(x => x.RoomId == roomId && x.UsersInRoom == new HashSet<User>());
            var firstUser = Mock.Of<User>(x => x.UserId == firstUserId && x.CurrentRoomId == roomId && x.Rooms == new HashSet<Room> {room});
            var secoundUser = Mock.Of<User>(x => x.UserId == secoundUserId && x.CurrentRoomId == 0 && x.Rooms == new HashSet<Room> {room});
            room.UsersInRoom.Add(firstUser);
            room.UsersInRoom.Add(secoundUser);
            _roomRepo.Setup(repo => repo.GetRoomById(It.IsAny<uint>())).Returns(room);
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(a => a == firstUserId))).Returns(firstUser);
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(a => a == secoundUserId))).Returns(secoundUser);
            var manager = new RoomManager(_roomRepo.Object,
                _userRepo.Object,
                _playlistRepo.Object);

            manager.DeleteUserFromRoom(firstUserId, roomId);
            manager.DeleteUserFromRoom(secoundUserId, roomId);

            Assert.IsFalse(room.UsersInRoom.Contains(firstUser));
            Assert.IsTrue(firstUser.CurrentRoomId == 0);
            Assert.IsFalse(room.UsersInRoom.Contains(secoundUser));
            Assert.IsTrue(secoundUser.CurrentRoomId == 0);
            _roomRepo.Verify(repo => repo.DeleteRoom(It.IsAny<Room>()));
        }

        [TestMethod]
        public void CahngeRoomAdmin_RoomAdminWasChanged()
        {
            var generator = new Random();
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var userId = (uint) generator.Next(1, int.MaxValue);
            var room = Mock.Of<Room>(x => x.RoomId == roomId && x.AdministratorId == 0);
            _roomRepo.Setup(repo => repo.GetRoomById(It.IsAny<uint>())).Returns(room);
            var manager = new RoomManager(_roomRepo.Object,
                _userRepo.Object,
                _playlistRepo.Object);

            manager.ChangeRoomAdmin(userId, room.RoomId);

            Assert.IsTrue(room.AdministratorId == userId);
            _roomRepo.Verify(repo => repo.UpdateRoom(It.IsAny<Room>()));
        }

        [TestMethod]
        public void StartPlayingSong_TheRoonHaveARecordAboutCurrentSong()
        {
            var generator = new Random();
            var songId = (uint) generator.Next(1, int.MaxValue);
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var room = Mock.Of<Room>(x => x.RoomId == roomId && x.CurrentSongId == 0 && x.CurrentSongStartTime == new DateTime());
            _roomRepo.Setup(repo => repo.GetRoomById(It.IsAny<uint>())).Returns(room);
            var manager = new RoomManager(_roomRepo.Object, 
                _userRepo.Object,
                _playlistRepo.Object);

            manager.StartPlayingSong(songId, roomId);

            Assert.IsTrue(room.CurrentSongId == songId);
            Assert.IsTrue(room.CurrentSongStartTime != new DateTime());
            _roomRepo.Verify(repo => repo.UpdateRoom(It.IsAny<Room>()));
        }
    }
}
