using Learnify.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Learnify.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);      // nếu Id là int -> Task<User?> GetByIdAsync(int id)
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);              // nếu Id là int -> Task DeleteAsync(int id)
        Task<int> CountAsync();
    }
}
