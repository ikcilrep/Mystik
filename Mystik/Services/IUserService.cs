using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(string nickname, string username, string password);
        Task<User> Retrieve(Guid id);
        Task Update(Guid id, User user);
        Task Delete(Guid id);
        Task<IEnumerable<User>> GetAll();
    }
}
