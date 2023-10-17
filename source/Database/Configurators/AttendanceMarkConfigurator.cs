using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Database.Configurators;

public class AttendanceMarkConfigurator : IEntityTypeConfiguration<AttendanceMark>
{
    public void Configure([NotNull] EntityTypeBuilder<AttendanceMark> builder)
    {
        builder.HasKey(entity => new { entity.ParticipantId, entity.EventId });
        builder.Property(entity => entity.ParticipantId).HasMaxLength(50);
        builder.Property(entity => entity.EventId).HasMaxLength(50);
    }
}
