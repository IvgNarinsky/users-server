using server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace server.Repositories.users
{
    public interface IUserRepository<T>
    {
        Task<GetUserResult> GetUserById(string id);
        Task<GetUserResult> GetAllUsers();
        Task<GetUserResult> AddUser(User entity);
        Task<GetUserResult> UpdateUser(User entity);
        Task<GetUserResult> DeleteUser(string id);
    }
}
