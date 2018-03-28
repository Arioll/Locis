using System;
using System.Collections.Generic;
using Common.Entities;
using DataAccess.Application;
using InvaitationMangment.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RoomMangment.Application;

namespace InvitationMangmentTest
{
    [TestClass]
    public class InvitationManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            _userRepo = new Mock<IUserRepository>();
            _roomRepo = new Mock<IRoomRepository>();
            _invitationRepo = new Mock<IInvitationRepository>();
            _roomManager = new Mock<IRoomManager>();
        }

        private Mock<IUserRepository> _userRepo;
        private Mock<IRoomRepository> _roomRepo;
        private Mock<IInvitationRepository> _invitationRepo;
        private Mock<IRoomManager> _roomManager;

        [TestMethod]
        public void InviteUserInRoom_UserWasInvited()
        {
            var generator = new Random();
            var firstUserId = (uint) generator.Next(1, int.MaxValue);
            var roomId = (uint)generator.Next(1, int.MaxValue);
            var senderId = (uint)generator.Next(1, int.MaxValue);
            var firstUser = Mock.Of<User>(x => 
                x.UserId == firstUserId && 
                x.Invaitations == new HashSet<Invitation>());
            var sender = Mock.Of<User>(x =>
                x.UserId == senderId &&
                x.Nickname == "sender");
            var room = Mock.Of<Room>(x => 
                x.RoomId == roomId && 
                x.UsersInRoom == new HashSet<User> {sender} && 
                x.RoomName == "testRoom");
            
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == firstUserId))).Returns(firstUser);
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == senderId))).Returns(sender);
            _roomRepo.Setup(repo => repo.GetRoomById(It.Is<uint>(id => id == roomId))).Returns(room);
            var manager = new InvitationManager(_invitationRepo.Object, _userRepo.Object, _roomRepo.Object, _roomManager.Object);

            manager.InviteUserInRoom(firstUserId, roomId, senderId);

            Assert.IsTrue(firstUser.Invaitations.Count == 1);
        }

        [TestMethod]
        public void InviteUserInroomSecoundTest_UserWasNotInvited()
        {
            var generator = new Random();
            var secoundUserId = (uint)generator.Next(1, int.MaxValue);
            var roomId = (uint)generator.Next(1, int.MaxValue);
            var senderId = (uint)generator.Next(1, int.MaxValue);
            var secoundUser = Mock.Of<User>(x => 
                x.UserId == secoundUserId && 
                x.Invaitations == new HashSet<Invitation>());
            var sender = Mock.Of<User>(x =>
                x.UserId == senderId &&
                x.Nickname == "sender");
            var room = Mock.Of<Room>(x => 
                x.RoomId == roomId && 
                x.UsersInRoom == new HashSet<User> {sender, secoundUser});
            
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == secoundUserId))).Returns(secoundUser);
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == senderId))).Returns(sender);
            _roomRepo.Setup(repo => repo.GetRoomById(It.Is<uint>(id => id == roomId))).Returns(room);
            var manager = new InvitationManager(_invitationRepo.Object, _userRepo.Object, _roomRepo.Object, _roomManager.Object);

            manager.InviteUserInRoom(secoundUserId, roomId, senderId);

            Assert.IsTrue(secoundUser.Invaitations.Count == 0);
        }

        [TestMethod]
        public void InviteUserInRoomThridTest_UserWasNotInvited()
        {
            var generator = new Random();
            var thridUserId = (uint) generator.Next(1, int.MaxValue);
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var senderId = (uint) generator.Next(1, int.MaxValue);
            var invitationId = (uint) generator.Next(1, int.MaxValue);
            var invitation = Mock.Of<Invitation>(x =>
                x.TargetId == thridUserId &&
                x.RoomId == roomId);
            var thridUser = Mock.Of<User>(x => 
                x.UserId == thridUserId && 
                x.Invaitations == new HashSet<Invitation> {invitation});
            var room = Mock.Of<Room>(x => 
                x.RoomId == roomId && 
                x.UsersInRoom == new HashSet<User>());
            var sender = Mock.Of<User>(x => 
                x.UserId == senderId && 
                x.Nickname == "sender");
            
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == thridUserId))).Returns(thridUser);
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == senderId))).Returns(sender);
            _roomRepo.Setup(repo => repo.GetRoomById(It.Is<uint>(id => id == roomId))).Returns(room);
            _invitationRepo.Setup(repo => repo.GetInvitationById(It.Is<uint>(id => id == invitationId)))
                .Returns(invitation);
            var manager = new InvitationManager(_invitationRepo.Object, _userRepo.Object, _roomRepo.Object, _roomManager.Object);

            manager.InviteUserInRoom(thridUserId, roomId, senderId);

            Assert.IsTrue(thridUser.Invaitations.Count == 1);
        }

        [TestMethod]
        public void DeleteInvitation_InvitationWasDeleted()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var invitationId = (uint)generator.Next(1, int.MaxValue);
            var invitation = Mock.Of<Invitation>(x => x.InvitationId == invitationId && x.TargetId == userId);
            var user = Mock.Of<User>(x => x.UserId == userId && x.Invaitations == new HashSet<Invitation> {invitation});
            _userRepo.Setup(repo => repo.GetUserById(It.Is<uint>(id => id == userId)))
                .Returns(user);
            _invitationRepo.Setup(repo => repo.GetInvitationById(It.Is<uint>(id => id == invitationId)))
                .Returns(invitation);
            var manager = new InvitationManager(_invitationRepo.Object, _userRepo.Object, _roomRepo.Object, _roomManager.Object);

            manager.DeleteInvitation(invitationId);

            Assert.IsTrue(user.Invaitations.Count == 0);
            _invitationRepo.Verify(repo => repo.DeleteInvitation(It.IsAny<Invitation>()));
        }

        [TestMethod]
        public void ResponseToInvitationTest_UserWasAddedToRoom()
        {
            var generator = new Random();
            var userId = (uint) generator.Next(1, int.MaxValue);
            var roomId = (uint) generator.Next(1, int.MaxValue);
            var invitationId = (uint) generator.Next(1, int.MaxValue);
            var invitation = Mock.Of<Invitation>(x => 
                x.TargetId == userId && 
                x.RoomId == roomId && 
                x.InvitationId == invitationId);
            var user = Mock.Of<User>(x => x.UserId == userId && x.Invaitations == new HashSet<Invitation> {invitation});
            _invitationRepo.Setup(x => x.GetInvitationById(It.Is<uint>(id => id == invitationId))).Returns(invitation);
            _userRepo.Setup(x => x.GetUserById(It.Is<uint>(id => id == userId))).Returns(user);
            var manager = new InvitationManager(_invitationRepo.Object, _userRepo.Object, _roomRepo.Object, _roomManager.Object);

            manager.ResponseToInvitation(invitationId, true);

            Assert.IsTrue(user.Invaitations.Count == 0);
            _userRepo.Verify(x => x.UpdateUser(user));
            _roomManager.Verify(x => x.AddUserInRoom(It.Is<uint>(id => id == userId), It.Is<uint>(id => id == roomId)));
            _invitationRepo.Verify(x => x.DeleteInvitation(It.IsAny<Invitation>()));
        }
    }
}
