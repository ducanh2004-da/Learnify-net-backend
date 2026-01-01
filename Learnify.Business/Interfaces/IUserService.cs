using Learnify.Common.DTOs;
using System.Threading.Tasks;

namespace Learnify.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserListResponse> GetAllUsersAsync();
        Task<UserResponse> GetUserByIdAsync(string id);
        Task<UserResponse> CreateUserAsync(CreateUserInput input);
        Task<UserResponse> UpdateUserAsync(UpdateUserInput input);
        Task<UserResponse> UpdateUserAdminAsync(UpdateUserAdminInput input);
        Task DeleteUserAsync(string id);
    }
}
