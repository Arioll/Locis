using Journalist;
using InvaitationMangment.Application;
using Common.Entities;
using DataAccess.Application;
using System.Collections.Generic;
using System.Linq;
using RoomMangment.Application;

namespace InvaitationMangment.Domain
{
    public class InvitationManager : IInvitationManager
    {
        public InvitationManager(IInvitationRepository invitationRepository,
            IUserRepository userRepository, 
            IRoomRepository roomRepository, 
            IRoomManager roomManager)
        {
            _invitationRepository = invitationRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _roomManager = roomManager;
        }

        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomManager _roomManager;

        public uint InviteUserInRoom(uint userId, uint roomId, uint senderId)
        {
            Require.Positive(userId, nameof(userId));
            Require.Positive(roomId, nameof(roomId));
            Require.Positive(senderId, nameof(senderId));
            
            var room = _roomRepository.GetRoomById(roomId);
            if (room.UsersInRoom.Any(x => x.UserId == userId)) return 0;
            var user = _userRepository.GetUserById(userId);
            if (user.Invaitations.Any(invite => invite.RoomId == roomId)) return 0;

            var sender = _userRepository.GetUserById(senderId).Nickname;
            var invitation = new Invitation(roomId, userId, room.RoomName, sender);
            var id = _invitationRepository.CreateInvitation(invitation);
            user.Invaitations.Add(_invitationRepository.GetInvitationById(id));
            _userRepository.UpdateUser(user);

            return id;
        }

        public Invitation GetInvitationById(uint invitationId)
        {
            Require.Positive(invitationId, nameof(invitationId));

            return _invitationRepository.GetInvitationById(invitationId);
        }

        public void ResponseToInvitation(uint invitationId, bool response)
        {
            Require.Positive(invitationId, nameof(invitationId));

            var invitation = _invitationRepository.GetInvitationById(invitationId);
            if (response)
            {
                _roomManager.AddUserInRoom(invitation.TargetId, invitation.RoomId);
            }
            DeleteInvitation(invitationId);
        }

        public void DeleteInvitation(uint invitationId)
        {
            Require.Positive(invitationId, nameof(invitationId));

            var invitation = _invitationRepository.GetInvitationById(invitationId);
            var user = _userRepository.GetUserById(invitation.TargetId);
            user.Invaitations.Remove(invitation);
            _userRepository.UpdateUser(user);
            _invitationRepository.DeleteInvitation(invitation);
        }

        public IEnumerable<Invitation> GetInvitationsByIds(IEnumerable<uint> ids)
        {
            Require.NotNull(ids, nameof(ids));

            return _invitationRepository.GetInvitationByIds(ids);
        }
    }
}