using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.Database.Configurators;

/// <summary>
/// Конфигуратор оценки мероприятия в рамках конкурса мероприятий, выставленной участником.
/// </summary>
public class ParticipantGradeConfigurator : IEntityTypeConfiguration<ParticipantGrade>
{
    /// <summary>
    /// Конфигурирует модель сущности базы данных.
    /// </summary>
    /// <param name="builder">Конструктор для конфигурирования модели.</param>
    public void Configure([NotNull] EntityTypeBuilder<ParticipantGrade> builder)
    {
        builder.HasKey(entity => new { entity.FeedbackId, entity.CriterionId });
        builder.Property(entity => entity.CriterionId).HasMaxLength(50);
    }
}
