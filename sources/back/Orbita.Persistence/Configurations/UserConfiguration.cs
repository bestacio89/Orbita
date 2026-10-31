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
        builder.Property(user => user.Email).HasMaxLength(320);
        builder.Property(user => user.FirstName).HasMaxLength(128);
        builder.Property(user => user.LastName).HasMaxLength(128);
        builder.Ignore(user => user.DisplayName);

        builder.HasMany(user => user.ExternalIdentities)
            .WithOne()
            .HasForeignKey(identity => identity.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(user => user.ExternalIdentities)
            .HasField("_externalIdentities")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
