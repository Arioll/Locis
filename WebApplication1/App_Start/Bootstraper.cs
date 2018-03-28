using System;
using System.Configuration;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Filters;
using AddFriendRequestMangment.Application;
using AddFriendRequestMangment.Domain;
using DataAccess.Application;
using DataAccess.NHibernate;
using DataAccess.Repositories;
using InvaitationMangment.Application;
using InvaitationMangment.Domain;
using RoomMangment.Application;
using RoomMangment.Domain;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SongMangment.Application;
using SongMangment.Domain;
using UserMangment.Application;
using UserMangment.Domain;
using EmailService.Application;
using EmailService.Models;
using EmailService.Domain;
using EmailService.Domain.MailFactorys;
using FileAccess.Application;
using FileAccess.Domain;
using PlaylistMangment.Application;
using PlaylistMangment.Domain;
using UserMangment.Domain.EmailOperations.Application;
using UserMangment.Domain.EmailOperations.Domain;
using WebApplication1.Filters;
using SimpleInjector.Lifestyles;
using SongMangment.Domain.SongFingerprintingSystem;

namespace WebApplication1
{
    public class Bootstraper
    {
        public void Setup()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            SetupDependencies(container);
            StartMessageSending(container);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            
        }

        private void SetupDependencies(Container container)
        {
            container.Register<IUserRepository, UserRepository>(Lifestyle.Singleton);
            container.Register<IRoomRepository, RoomRepository>(Lifestyle.Singleton);
            container.Register<IInvitationRepository, InvitationRepository>(Lifestyle.Singleton);
            container.Register<IUserManager, UserManager>(Lifestyle.Singleton);
            container.Register<IRoomManager, RoomManager>(Lifestyle.Singleton);
            container.Register<ISongManager, SongManager>(Lifestyle.Singleton);
            container.Register<ISongRepository, SongRepository>(Lifestyle.Singleton);
            container.Register<IInvitationManager, InvitationManager>(Lifestyle.Singleton);
            container.Register<IAuthenticationFilter, AuthenticationFilter>(Lifestyle.Singleton);
            container.Register<IPlaylistRepository, PlaylistRepository>(Lifestyle.Singleton);
            container.Register<IPlaylistManager, PlaylistManager>(Lifestyle.Singleton);
            container.Register<IFileRepository, FileRepository>(Lifestyle.Singleton);
            container.Register<IAddFriendRequestManager, AddFriendRequestManager>(Lifestyle.Singleton);
            container.Register<IAddToFriendRequestRepository, AddToFriendRequestRepository>(Lifestyle.Singleton);
            container.Register(() => new SessionProvider(), Lifestyle.Singleton);
            container.Register(() => new SongsFingerprinter(), Lifestyle.Singleton);
            container.Register<IEmailConfirmation>(() => new EmailConfirmation(
                container.GetInstance<IMailSendManager>(),
                TimeSpan.FromSeconds(7200), 
                container.GetInstance<IUserManager>()), Lifestyle.Singleton);
            container.Register<ISmtpClientFactory>(() => new SmtpClientFactory(new MailCredentials(
                ConfigurationManager.ConnectionStrings["LocisEmail"].ConnectionString,
                ConfigurationManager.ConnectionStrings["LocisEmailPassword"].ConnectionString,
                ConfigurationManager.ConnectionStrings["SmtpServer"].ConnectionString,
                int.Parse(ConfigurationManager.ConnectionStrings["SmtpPort"].ConnectionString))), Lifestyle.Singleton);
            container.Register<IEmailMessageRepository>(() => 
                new EmailMessageRepository(container.GetInstance<SessionProvider>()), Lifestyle.Singleton);
            container.Register<IMailSendManager, MailSendManager>(Lifestyle.Singleton);
            container.Register(() => new MailSender(
                container.GetInstance<ISmtpClientFactory>(), 
                container.GetInstance<IEmailMessageRepository>(),
                new CancellationTokenSource(),
                new MailBuildingDirector()), Lifestyle.Singleton);
            container.Register<IAuthorization>(() => new UserMangment.Domain.Authorization.Authorization(
                TimeSpan.FromSeconds(1800),
                container.GetInstance<IUserRepository>()),
                Lifestyle.Singleton);
        }

        private void StartMessageSending(Container container)
        {
            var sender = container.GetInstance<MailSender>();
            sender.StartSending();
        }
    }
}