using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;
using Mystik.Models.User;

namespace Mystik.Services
{
    public interface IUserService : IDisposable
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(string nickname, string username, string password);
        Task<User> Retrieve(Guid id);
        Task Update(Guid id, Entities.User user);
        Task Update(Guid id, Patch model);
        Task Delete(Guid id);
        Task AddFriend(Guid inviterId, Guid invitedId);
        Task DeleteFriends(Guid id, List<Guid> usersIds);
        Task InviteFriends(Guid id, List<Guid> usersIds);
        Task DeleteInvitations(Guid id, List<Guid> usersIds);
        Task<bool> IsUserInvited(Guid inviterId, Guid invitedId);
        Task<IEnumerable<User>> GetAll();
    }
}
