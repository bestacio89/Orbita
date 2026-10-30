using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
{
    public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
    {
        builder.ToTable("ProjectAssignments");
        builder.HasKey(assignment => assignment.Id);
        builder.HasIndex(assignment => new { assignment.ProjectId, assignment.UserId }).IsUnique();
        builder.HasIndex(assignment => assignment.UserId);
        builder.Property(assignment => assignment.Role).HasConversion<int>();
    }
}
