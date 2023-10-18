using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.DataAccess.Migrations;

/// <summary>
/// Миграция базы данных, создающая таблицу голосов участников в рамках зрительских голосований.
/// </summary>
public partial class ParticipantVotesCreation : Migration
{
    /// <summary>
    /// Применяет миграцию к базе данных.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ParticipantVotes",
            columns: table => new
            {
                Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                VoitingParticipantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantVotes", x => x.Identifier);
            });
    }

    /// <summary>
    /// Откатывает ранее применённую к базе данных миграцию.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ParticipantVotes");
    }
}
