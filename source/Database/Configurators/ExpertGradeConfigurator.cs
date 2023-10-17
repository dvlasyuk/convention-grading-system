using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Database.Configurators;

/// <summary>
/// Конфигуратор оценки мероприятия в рамках конкурса мероприятий, выставленной экспертом.
/// </summary>
public class ExpertGradeConfigurator : IEntityTypeConfiguration<ExpertGrade>
{
    /// <summary>
    /// Конфигурирует модель сущности базы данных.
    /// </summary>
    /// <param name="builder">Конструктор для конфигурирования модели.</param>
    public void Configure([NotNull] EntityTypeBuilder<ExpertGrade> builder)
    {
        builder.HasKey(entity => entity.Identifier);
        builder.Property(entity => entity.CriterionId).HasMaxLength(50);
    }
}

