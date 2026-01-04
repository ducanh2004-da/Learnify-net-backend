using Learnify.Common.DTOs;
using System.Threading.Tasks;

namespace Learnify.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserListResponse> GetAllUsersAsync();
        Task<UserListResponse> GetUserByIdAsync(string id);
        Task<UserListResponse> CreateUserAsync(CreateUserInput input);
        Task<UserListResponse> UpdateUserAsync(UpdateUserInput input);
        Task<UserListResponse> UpdateUserAdminAsync(UpdateUserAdminInput input);
        Task<UserListResponse> DeleteUserAsync(string id);
    }
}
