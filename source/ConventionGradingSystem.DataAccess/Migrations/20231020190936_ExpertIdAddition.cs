using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.DataAccess.Migrations;

/// <summary>
/// Миграция базы данных, добавляющая идентификатор эксперта к отзывам экспертов.
/// </summary>
public partial class ExpertIdAddition : Migration
{
    /// <summary>
    /// Применяет миграцию к базе данных.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ExpertId",
            table: "ExpertFeedbacks",
            type: "TEXT",
            maxLength: 50,
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
            name: "ExpertId",
            table: "ExpertFeedbacks");
    }
}
