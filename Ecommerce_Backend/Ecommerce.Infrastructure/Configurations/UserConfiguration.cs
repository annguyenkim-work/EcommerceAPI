using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(u => u.Email).IsUnique(); // Đảm bảo không trùng Email
            builder.HasIndex(u => u.IsDeleted);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

            // Cấu hình Filter cho Soft Delete
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
