using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.DataAccess.Database.Configurators;

/// <summary>
/// Конфигуратор отзыва эксперта на мероприятие в рамках конкурса мероприятий.
/// </summary>
public class ExpertFeedbackConfigurator : IEntityTypeConfiguration<ExpertFeedback>
{
    /// <summary>
    /// Конфигурирует модель сущности базы данных.
    /// </summary>
    /// <param name="builder">Конструктор для конфигурирования модели.</param>
    public void Configure([NotNull] EntityTypeBuilder<ExpertFeedback> builder)
    {
        builder.HasKey(entity => entity.Identifier);
        builder
            .HasMany(entity => entity.Grades)
            .WithOne(entity => entity.Feedback)
            .HasForeignKey(entity => entity.FeedbackId);

        builder.Property(entity => entity.EventId).HasMaxLength(50);
        builder.Property(entity => entity.ExpertId).HasMaxLength(50);
        builder.Property(entity => entity.Note).HasMaxLength(1000);
    }
}
