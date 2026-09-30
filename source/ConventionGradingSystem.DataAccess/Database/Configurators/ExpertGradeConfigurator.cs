using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.DataAccess.Database.Configurators;

public class ExpertGradeConfigurator : IEntityTypeConfiguration<ExpertGrade>
{
    public void Configure([NotNull] EntityTypeBuilder<ExpertGrade> builder)
    {
        builder.HasKey(entity => new { entity.FeedbackId, entity.CriterionId });
        builder.Property(entity => entity.CriterionId).HasMaxLength(50);
    }
}

