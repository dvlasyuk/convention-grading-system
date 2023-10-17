using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Database.Configurators;

public class ExpertGradeConfigurator : IEntityTypeConfiguration<ExpertGrade>
{
    public void Configure([NotNull] EntityTypeBuilder<ExpertGrade> builder)
    {
        builder.HasKey(entity => entity.Identifier);
        builder.Property(entity => entity.CriterionId).HasMaxLength(50);
    }
}

