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
                return new UserListResponse
                {
                    IsSuccess = false,
                    Users = new List<UserResponse>(),
                    Count = 0,
                    Message = "Users retrieved unsuccessfully"
                };
            }

            return new UserListResponse
            {
                IsSuccess = true,
                Users = users.Select(MapToResponse).ToList(),
                Count = count,
                Message = "Users retrieved successfully"
            };
        }

        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            return MapToResponse(user);
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserInput input)
        {
            var existingUser = await _userRepository.GetByEmailAsync(input.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);

            var user = new User
            {
                Username = input.Username,
                Email = input.Email,
                Password = hashedPassword,
                PhoneNumber = input.PhoneNumber,
            };

            // parse role if provided (safely)
            if (!string.IsNullOrWhiteSpace(input.Role))
            {
                if (!Enum.TryParse<Role>(input.Role, true, out var parsedRole))
                    throw new ArgumentException("Invalid role value", nameof(input.Role));
                user.Role = parsedRole;
            }

            // call repository to add and save
            await _userRepository.CreateAsync(user);

            // return the created user's DTO (user.Id should be set after SaveChanges)
            return MapToResponse(user);
        }

        public async Task<UserResponse> UpdateUserAsync(UpdateUserInput input)
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

            return MapToResponse(user);
        }

        public async Task<UserResponse> UpdateUserAdminAsync(UpdateUserAdminInput input)
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

            if (!string.IsNullOrWhiteSpace(input.Role))
            {
                if (!Enum.TryParse<Role>(input.Role, true, out var parsedRole))
                    throw new ArgumentException("Invalid role value", nameof(input.Role));
                user.Role = parsedRole;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return MapToResponse(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            if (user.Role == Role.ADMIN)
                throw new InvalidOperationException("Cannot delete an ADMIN user");

            await _userRepository.DeleteAsync(id);
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
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
