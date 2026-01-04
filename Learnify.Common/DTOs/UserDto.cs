using Learnify.Common.Enums;
using System;
using System.Collections.Generic;

namespace Learnify.Common.DTOs
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserListResponse
    {
        public bool IsSuccess { get; set; }
        // <-- Sửa ở đây: dùng List<UserResponse> (DTO), không phải List<User> (entity)
        public List<UserResponse>? Users { get; set; } = new();
        public int? Count { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CreateUserInput
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string Role { get; set; } = "USER";
    }

    public class UpdateUserInput
    {
        public string Id { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
    }

    public class UpdateUserAdminInput : UpdateUserInput
    {
        public string? Role { get; set; } = "USER";
    }
}
