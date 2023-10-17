using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Host.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Host.Database.Configurators;

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
        builder.HasKey(entity => new { entity.FeedbackId, entity.CriterionId });
        builder.Property(entity => entity.CriterionId).HasMaxLength(50);
    }
}

