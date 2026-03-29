namespace Ecommerce.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Avatar { get; set; }

        // Social Login
        public string? GoogleId { get; set; }
        public string? FacebookId { get; set; }

        // Lockout & Security
        public bool IsActive { get; set; } = true;
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } // Soft delete
    }
}
