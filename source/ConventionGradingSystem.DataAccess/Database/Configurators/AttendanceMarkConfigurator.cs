using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.DataAccess.Database.Configurators;

/// <summary>
/// Конфигуратор отметки о посещении участником мероприятия в рамках конкурса мероприятий.
/// </summary>
public class AttendanceMarkConfigurator : IEntityTypeConfiguration<AttendanceMark>
{
    /// <summary>
    /// Конфигурирует модель сущности базы данных.
    /// </summary>
    /// <param name="builder">Конструктор для конфигурирования модели.</param>
    public void Configure([NotNull] EntityTypeBuilder<AttendanceMark> builder)
    {
        builder.HasKey(entity => new { entity.ParticipantId, entity.EventId });
        builder.Property(entity => entity.ParticipantId).HasMaxLength(50);
        builder.Property(entity => entity.EventId).HasMaxLength(50);
    }
}
