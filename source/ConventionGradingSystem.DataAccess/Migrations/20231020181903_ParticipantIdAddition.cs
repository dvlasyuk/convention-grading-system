using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.DataAccess.Migrations;

/// <summary>
/// Миграция базы данных, добавляющая идентификатор участника к отзывам участников.
/// </summary>
public partial class ParticipantIdAddition : Migration
{
    /// <summary>
    /// Применяет миграцию к базе данных.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ParticipantId",
            table: "ParticipantFeedbacks",
            type: "TEXT",
            nullable: false,
            defaultValue: "");
    }

    /// <summary>
    /// Откатывает ранее применённую к базе данных миграцию.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ParticipantId",
            table: "ParticipantFeedbacks");
    }
}
