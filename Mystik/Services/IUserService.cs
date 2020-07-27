using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Services
{
    public interface IUserService : IDisposable
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(string nickname, string username, string password);
        Task<User> Retrieve(Guid id);
        Task<IReadOnlyList<string>> Update(Guid id, string newNickname, string newPassword);
        Task<IReadOnlyList<string>> Delete(Guid id);
        Task AddFriend(Guid inviterId, Guid invitedId);
        Task DeleteFriends(Guid id, List<Guid> usersIds);
        Task<IReadOnlyList<string>> InviteFriends(Guid inviterId, List<Guid> invitedIds);
        Task<UserRelatedEntities> GetNotExisting(Guid id, UserRelatedEntities entities);
        Task DeleteInvitations(Guid inviterId, List<Guid> invitedIds);
        Task<bool> IsUserInvited(Guid inviterId, Guid invitedId);
        Task<IEnumerable<User>> GetAll();
    }
}
