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
        Task AddFriends(Guid id, List<Guid> usersIds);
        Task DeleteFriends(Guid id, List<Guid> usersIds);
        Task InviteFriends(Guid id, List<Guid> usersIds);
        Task DeleteInvitations(Guid id, List<Guid> usersIds);
        Task<IEnumerable<User>> GetAll();
    }
}
