using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.DataAccess.Migrations;

/// <summary>
/// Миграция базы данных, изменяющая названия полей, связанный с сущностью кандидата.
/// </summary>
public partial class CandidateRenaming : Migration
{
    /// <summary>
    /// Применяет миграцию к базе данных.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "VoitingParticipantId",
            table: "ParticipantVotes",
            newName: "CandidateId");
    }

    /// <summary>
    /// Откатывает ранее применённую к базе данных миграцию.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "CandidateId",
            table: "ParticipantVotes",
            newName: "VoitingParticipantId");
    }
}
