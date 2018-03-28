using System;
using System.Collections.Generic;
using System.Net.Mail;
using Common.Entities;
using DataAccess.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserMangment.Domain.Authorization;

namespace UserMangmentTest
{
    [TestClass]
    public class AuthorizationTest
    {
        [TestInitialize]
        public void Setup()
        {
            _userRepo = new Mock<IUserRepository>();
        }

        private Mock<IUserRepository> _userRepo;
        
        [TestMethod]
        public void Authorize_AuthorizationComplete()
        {
            var generator = new Random();
            var userId = generator.Next(1, int.MaxValue);
            var user = Mock.Of<User>(x => x.UserId == userId && 
                x.Email == new MailAddress("testMail@mail.ru") && 
                x.Password == new Password("testPass"));
            _userRepo.Setup(repo => repo.GetUsersByPredicate(It.IsAny<Func<User, bool>>())).Returns(new List<User> {user});
            var authorization = new Authorization(TimeSpan.FromSeconds(5), _userRepo.Object);

            var token = authorization.Autorize("testMail@mail.ru", new Password("testPass"));

            Assert.IsTrue(token != null);
        }
    }
}
