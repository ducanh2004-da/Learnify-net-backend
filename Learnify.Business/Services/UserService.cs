using Learnify.Common.DTOs;
using Learnify.Business.Interfaces;
using Learnify.Repository.Interfaces;
using Learnify.Repository.Models;
using Learnify.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Learnify.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserListResponse> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var count = await _userRepository.CountAsync();

            if (users == null || !users.Any())
            {
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "Users retrieved unsuccessfully");
            }

            var usersDto = users.Select(MapToResponse).ToList();
            return MapToListResponse(true, usersDto, "Users retrieved successfully");
        }

        public async Task<UserListResponse> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "User retrieved unsuccessfully");

            var dto = MapToResponse(user);
            return MapToListResponse(true, dto, "User retrieved successfully");
        }

        public async Task<UserListResponse> CreateUserAsync(CreateUserInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) ||
                string.IsNullOrWhiteSpace(input.Email) ||
                string.IsNullOrWhiteSpace(input.Password))
            {
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "Username, email and password are required");
            }

            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(input.Email);
                if (existingUser != null)
                    return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "Email already exist");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);

                var avatarValue = input.Avatar ?? string.Empty;
                var addressValue = input.Address ?? string.Empty;
                var phoneValue = input.PhoneNumber ?? string.Empty;

                var user = new User
                {
                    // ✅ GENERATE ID TRƯỚC KHI LƯU
                    Id = Guid.NewGuid().ToString(), // Hoặc ObjectId.GenerateNewId().ToString() nếu MongoDB

                    Username = input.Username,
                    Email = input.Email,
                    Password = hashedPassword,
                    PhoneNumber = phoneValue,
                    Address = addressValue,
                    Avatar = avatarValue,
                    Role = input.Role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                

                // ✅ KHÔNG GÁN - CHỈ AWAIT
                await _userRepository.CreateAsync(user);

                // ✅ Sử dụng user đã có Id (đã generate ở trên)
                // Không cần GetByEmailAsync nữa vì user đã có đầy đủ thông tin
                var createdDto = MapToResponse(user);
                return MapToListResponse(true, createdDto, "User created successfully");
            }
            catch (Exception ex)
            {
                // Log chi tiết để debug
                Console.WriteLine($"Error creating user: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), $"Internal server error: {ex.Message}");
            }
        }



        public async Task<UserListResponse> UpdateUserAsync(UpdateUserInput input)
        {
            var user = await _userRepository.GetByIdAsync(input.Id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {input.Id} not found");

            if (!string.IsNullOrEmpty(input.Email))
            {
                var emailExists = await _userRepository.GetByEmailAsync(input.Email);
                if (emailExists != null && emailExists.Id != input.Id)
                    throw new InvalidOperationException("Email already in use by another user");
            }

            if (!string.IsNullOrEmpty(input.Username)) user.Username = input.Username;
            if (!string.IsNullOrEmpty(input.Email)) user.Email = input.Email;
            if (input.PhoneNumber != null) user.PhoneNumber = input.PhoneNumber;
            if (input.Address != null) user.Address = input.Address;
            if (input.Avatar != null) user.Avatar = input.Avatar;

            // update metadata
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // trả về dưới dạng list chứa 1 phần tử
            var dto = MapToResponse(user);
            return MapToListResponse(true, dto, "User updated successfully");
        }

        public async Task<UserListResponse> UpdateUserAdminAsync(UpdateUserAdminInput input)
        {
            var user = await _userRepository.GetByIdAsync(input.Id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {input.Id} not found");

            if (!string.IsNullOrEmpty(input.Email))
            {
                var emailExists = await _userRepository.GetByEmailAsync(input.Email);
                if (emailExists != null && emailExists.Id != input.Id)
                    return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "Email already in use by another user");
            }

            if (!string.IsNullOrEmpty(input.Username)) user.Username = input.Username;
            if (!string.IsNullOrEmpty(input.Email)) user.Email = input.Email;
            if (input.PhoneNumber != null) user.PhoneNumber = input.PhoneNumber;
            if (input.Address != null) user.Address = input.Address;
            if (input.Avatar != null) user.Avatar = input.Avatar;

            if (!string.IsNullOrWhiteSpace(input.Role))
            {
                if (!Enum.TryParse<Role>(input.Role, true, out var parsedRole))
                    throw new ArgumentException("Invalid role value", nameof(input.Role));
                user.Role = input.Role;
            }

                user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var dto = MapToResponse(user);
            return MapToListResponse(true, dto, "Update user successfully");
        }

        public async Task<UserListResponse> DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "User not found");

            if (user.Role == "ADMIN")
                return MapToListResponse(false, Enumerable.Empty<UserResponse>(), "Cannot delete an admin user");

            await _userRepository.DeleteAsync(id);
            return MapToListResponse(true, Enumerable.Empty<UserResponse>(), "User deleted successfully");
        }

        private UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role.ToString(),
                Avatar = user.Avatar ?? string.Empty,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // overload: nhận IEnumerable<UserResponse>
        private UserListResponse MapToListResponse(bool isSuccess, IEnumerable<UserResponse>? users, string message = "")
        {
            var list = users?.ToList() ?? new List<UserResponse>();
            return new UserListResponse
            {
                IsSuccess = isSuccess,
                Users = list,
                Count = list.Count,
                Message = message ?? string.Empty
            };
        }

        // overload: nhận single UserResponse (tiện khi chỉ có 1 user)
        private UserListResponse MapToListResponse(bool isSuccess, UserResponse? user, string message = "")
        {
            var list = user is null ? new List<UserResponse>() : new List<UserResponse> { user };
            return new UserListResponse
            {
                IsSuccess = isSuccess,
                Users = list,
                Count = list.Count,
                Message = message ?? string.Empty
            };
        }
    }
}
