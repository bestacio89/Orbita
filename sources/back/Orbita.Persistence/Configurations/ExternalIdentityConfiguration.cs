using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
    {
        builder.ToTable("ExternalIdentities");
        builder.HasKey(identity => identity.Id);
        builder.Property(identity => identity.ProviderKind).HasConversion<int>().IsRequired();
        builder.Property(identity => identity.Issuer).HasMaxLength(512).IsRequired();
        builder.Property(identity => identity.Subject).HasMaxLength(512).IsRequired();
        builder.HasIndex(identity => new { identity.Issuer, identity.Subject })
            .IsUnique()
            .HasDatabaseName("IX_ExternalIdentities_Issuer_Subject");
        builder.HasIndex(identity => identity.UserId)
            .HasDatabaseName("IX_ExternalIdentities_UserId");
    }
}
