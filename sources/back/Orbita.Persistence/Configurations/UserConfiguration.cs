using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.HasIndex(user => new { user.TenantId, user.ObjectId }).IsUnique();
        builder.Property(user => user.Email).HasMaxLength(320);
        builder.Property(user => user.FirstName).HasMaxLength(128);
        builder.Property(user => user.LastName).HasMaxLength(128);
        builder.Ignore(user => user.DisplayName);
        // External identity persistence is introduced with its dedicated schema migration slice.
        builder.Ignore(user => user.ExternalIdentities);
    }
}
