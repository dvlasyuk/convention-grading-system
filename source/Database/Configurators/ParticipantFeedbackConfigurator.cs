using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Database.Configurators;

public class ParticipantFeedbackConfigurator : IEntityTypeConfiguration<ParticipantFeedback>
{
    public void Configure([NotNull] EntityTypeBuilder<ParticipantFeedback> builder)
    {
        builder.HasKey(entity => entity.Identifier);
        builder
            .HasMany(entity => entity.Grades)
            .WithOne(entity => entity.Feedback)
            .HasForeignKey(entity => entity.FeedbackId);

        builder.Property(entity => entity.EventId).HasMaxLength(50);
        builder.Property(entity => entity.Note).HasMaxLength(1000);
    }
}
