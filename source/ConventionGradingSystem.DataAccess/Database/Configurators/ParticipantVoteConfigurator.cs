using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConventionGradingSystem.DataAccess.Database.Configurators;

/// <summary>
/// Конфигуратор голоса участника в рамках зрительского голосования.
/// </summary>
public class ParticipantVoteConfigurator : IEntityTypeConfiguration<ParticipantVote>
{
    /// <summary>
    /// Конфигурирует модель сущности базы данных.
    /// </summary>
    /// <param name="builder">Конструктор для конфигурирования модели.</param>
    public void Configure([NotNull] EntityTypeBuilder<ParticipantVote> builder)
    {
        builder.HasKey(entity => entity.Identifier);
        builder.Property(entity => entity.ParticipantId).HasMaxLength(50);
        builder.Property(entity => entity.CandidateId).HasMaxLength(50);
        builder.Property(entity => entity.Note).HasMaxLength(1000);
    }
}
