using System;
using System.Threading.Tasks;
using Mystik.Entities;

namespace Mystik.Data
{
    public interface IUserRepository
    {
        Task<User> Create(string username, string password);
        Task<User> Retrieve(Guid id);
        Task Update(Guid id, User user);
        Task Delete(Guid id);
    }
}
