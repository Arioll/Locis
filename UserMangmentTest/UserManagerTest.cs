using System;
using System.Collections.Generic;
using System.Linq;
using Common.Entities;
using DataAccess.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RoomMangment.Application;
using UserMangment.Application;
using UserMangment.Domain;

namespace UserMangmentTest
{
    [TestClass]
    public class UserManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            _userRepo = new Mock<IUserRepository>();
            _roomRepo = new Mock<IRoomRepository>();
            _authorization = new Mock<IAuthorization>();
            _roomManager = new Mock<IRoomManager>();
            _invitationRepository = new Mock<IInvitationRepository>();
            _playlistRepo = new Mock<IPlaylistRepository>();
            _addToFriendRequestRepo = new Mock<IAddToFriendRequestRepository>();
        }

        private Mock<IUserRepository> _userRepo;
        private Mock<IRoomRepository> _roomRepo;
        private Mock<IAuthorization> _authorization;
        private Mock<IRoomManager> _roomManager;
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IPlaylistRepository> _playlistRepo;
        private Mock<IAddToFriendRequestRepository> _addToFriendRequestRepo;

        [TestMethod]
        public void DeleteUser_UserWasDeleted()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var uncreatedUserIdFirst = (uint) generator.Next(1, int.MaxValue);
            var uncreatedUserIdSecound = (uint) generator.Next(1, int.MaxValue);
            var firstRoomId = (uint) generator.Next(1, int.MaxValue);
            var secoundRoomId = (uint) generator.Next(1, int.MaxValue);
            var userPlaylistId = (uint) generator.Next(1, int.MaxValue);
            var firstUser = Mock.Of<User>(x => x.UserId == uncreatedUserIdFirst);
            var secoundUser = Mock.Of<User>(x => x.UserId == uncreatedUserIdSecound);
            var firstRoom = Mock.Of<Room>(x =>
                x.UsersInRoom == new HashSet<User> { firstUser } &&
                x.RoomId == firstRoomId &&
                x.AdministratorId == userId);
            var secoundRoom = Mock.Of<Room>(x =>
                x.UsersInRoom == new HashSet<User> { secoundUser } &&
                x.RoomId == secoundRoomId &&
                x.AdministratorId == uncreatedUserIdSecound);
            var user = Mock.Of<User>(x =>
                x.IsDeleted == false &&
                x.UserId == userId &&
                x.Rooms == new HashSet<Room> {firstRoom, secoundRoom} &&
                x.Invaitations == new HashSet<Invitation>() &&
                x.PlaylistId == userPlaylistId &&
                x.Friends == new HashSet<User>());
            firstRoom.UsersInRoom.Add(user);
            secoundRoom.UsersInRoom.Add(user);
            
            var userPlaylist = new Playlist();
            _userRepo.Setup(repo => repo.GetUserById(It.IsAny<uint>())).Returns(user);
            _roomRepo.Setup(repo => repo.GetRoomById(It.Is<uint>(id => id == firstRoomId))).Returns(firstRoom);
            _roomRepo.Setup(repo => repo.GetRoomById(It.Is<uint>(id => id == secoundRoomId))).Returns(secoundRoom);
            _playlistRepo.Setup(repo => repo.GetPlaylistById(It.Is<uint>(id => id == userPlaylistId)))
                .Returns(userPlaylist);
            var userManager = new UserManager(_userRepo.Object,
                _roomRepo.Object,
                _authorization.Object,
                _roomManager.Object,
                _invitationRepository.Object,
                _playlistRepo.Object,
                _addToFriendRequestRepo.Object);

            userManager.DeleteUser(userId);

            Assert.IsTrue(firstRoom.AdministratorId == uncreatedUserIdFirst);
            Assert.IsTrue(secoundRoom.AdministratorId == uncreatedUserIdSecound);
            Assert.IsTrue(user.IsDeleted);
            Assert.IsTrue(userPlaylist.Count == 0);
            _playlistRepo.Verify(repo => repo.UpdatePlaylist(It.IsAny<Playlist>()));
            _userRepo.Verify(repo => repo.UpdateUser(It.IsAny<User>()));
        }

        [TestMethod]
        public void DeleteUserTestPart2_AllUserFriandsWasDeleted()
        {
            var generator = new Random();
            var userId = (uint)generator.Next(1, int.MaxValue);
            var friendsCount = generator.Next(10, 1000);
            var userSet = new HashSet<User>();
            var mainUser = Mock.Of<User>(x => x.UserId == userId &&
                x.Friends == userSet &&
                x.Invaitations == new HashSet<Invitation>() &&
                x.Rooms == new HashSet<Room>());
            for (var i = 0; i < friendsCount; i++)
            {
                var friendId = (uint) generator.Next(1, int.MaxValue);
                var user = Mock.Of<User>(x => x.UserId == friendId && 
                x.Friends == new HashSet<User> {mainUser});
                userSet.Add(user);
            }
            
            _addToFriendRequestRepo.Setup(x => x.GetRequestsByPredicate(It.IsAny<Func<AddToFriendRequest, bool>>()))
                .Returns(new List<AddToFriendRequest>());
            _playlistRepo.Setup(x => x.GetPlaylistById(It.IsAny<uint>())).Returns(new Playlist());
            foreach (var id in userSet)
            {
                _userRepo.Setup(x => x.GetUserById(It.Is<uint>(usid => usid == id.UserId))).Returns(userSet.First(x => x.UserId == id.UserId));
            }
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(id => id == userId))).Returns(mainUser);
            var userManager = new UserManager(_userRepo.Object,
                _roomRepo.Object,
                _authorization.Object,
                _roomManager.Object,
                _invitationRepository.Object,
                _playlistRepo.Object,
                _addToFriendRequestRepo.Object);

            userManager.DeleteUser(userId);

            Assert.IsTrue(mainUser.Friends.Count == 0);
        }

        [TestMethod]
        public void ResponseToRequestTest()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var friendId = (uint) generator.Next(1, int.MaxValue);
            var request = new AddToFriendRequest(userId, friendId, "some name");
            var user = Mock.Of<User>(x => x.UserId == userId && x.Friends == new HashSet<User>());
            var friend = Mock.Of<User>(x => x.UserId == friendId && 
                x.Friends == new HashSet<User>() && 
                x.AddToFriendRequests == new HashSet<AddToFriendRequest> {request});
            
            var userManager = new UserManager(_userRepo.Object,
                _roomRepo.Object,
                _authorization.Object,
                _roomManager.Object,
                _invitationRepository.Object,
                _playlistRepo.Object,
                _addToFriendRequestRepo.Object);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(y => y == userId))).Returns(user);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(y => y == friendId))).Returns(friend);
            _addToFriendRequestRepo.Setup(x => x.GetRequestById(It.IsAny<uint>())).Returns(request);

            userManager.ResponseToRequest(1234, true);

            Assert.IsTrue(user.Friends.Contains(friend));
            Assert.IsTrue(friend.Friends.Contains(user));
            Assert.IsTrue(friend.AddToFriendRequests.Count == 0);
        }

        [TestMethod]
        public void DeleteFriendTest()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var friendId = (uint) generator.Next(1, int.MaxValue);
            var user = Mock.Of<User>(x => x.UserId == userId && x.Friends == new HashSet<User> {});
            var friend = Mock.Of<User>(x => x.UserId == friendId && x.Friends == new HashSet<User> {user});
            user.Friends.Add(friend);
            var userManager = new UserManager(_userRepo.Object,
                _roomRepo.Object,
                _authorization.Object,
                _roomManager.Object,
                _invitationRepository.Object,
                _playlistRepo.Object,
                _addToFriendRequestRepo.Object);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(y => y == userId))).Returns(user);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(y => y == friendId))).Returns(friend);
            _userRepo.Setup(x => x.UpdateUser(It.IsAny<User>()));

            userManager.DeleteFriend(userId, friendId);
            
            _userRepo.Verify(x => x.UpdateUser(friend));
            Assert.IsFalse(friend.Friends.Contains(user));
        }

        [TestMethod]
        public void SendRequestToAddInFriendsTest()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var friendId = (uint) generator.Next(1, int.MaxValue);
            var requestId = (uint) generator.Next(1, int.MaxValue);
            var user = Mock.Of<User>(x => x.UserId == userId && x.Nickname == "testNickname");
            var friend = Mock.Of<User>(x => x.UserId == friendId && x.AddToFriendRequests == new HashSet<AddToFriendRequest>());
            var userManager = new UserManager(_userRepo.Object,
                _roomRepo.Object,
                _authorization.Object,
                _roomManager.Object,
                _invitationRepository.Object,
                _playlistRepo.Object,
                _addToFriendRequestRepo.Object);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(id => id == userId))).Returns(user);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(id => id == friendId))).Returns(friend);
            _addToFriendRequestRepo.Setup(x => x.SaveRequest(It.IsAny<AddToFriendRequest>())).Returns(requestId);

            userManager.SendRequestToAddNewFriend(userId, friendId);

            Assert.IsTrue(friend.AddToFriendRequests.Count == 1);
        }
    }
}