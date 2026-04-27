using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learnify.Common.Enums;

namespace Learnify.Repository.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = "Anonymous";
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? GoogleId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "USER";
        public string? HashedRefreshToken { get; set; }
        public string? Avatar { get; set; }
        public bool? IsVerified { get; set; }
        public int? CurrentSteak {  get; set; }
        public int? LongestSteak { get; set; }
        public int? Diamond {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool? IsDeleted { get; set; }
        public DateTime? DeleteAt { get; set; }

        public virtual ICollection<DiamondTransaction> DiamondTransactions { get; set; } = new List<DiamondTransaction>();
    }

    public class DiamondTransaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public DiamondTransactionType Type { get; set; }
        public DiamondSource Source { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
    }


}
