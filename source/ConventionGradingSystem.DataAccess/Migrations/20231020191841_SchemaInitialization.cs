using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.DataAccess.Migrations;

/// <summary>
/// Миграция базы данных, инициализирующая схему.
/// </summary>
public partial class SchemaInitialization : Migration
{
    /// <summary>
    /// Применяет миграцию к базе данных.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AttendanceMarks",
            columns: table => new
            {
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                EventId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                SpecialMark = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AttendanceMarks", x => new { x.ParticipantId, x.EventId });
            });

        migrationBuilder.CreateTable(
            name: "ExpertFeedbacks",
            columns: table => new
            {
                Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                EventId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                ExpertId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExpertFeedbacks", x => x.Identifier);
            });

        migrationBuilder.CreateTable(
            name: "ParticipantFeedbacks",
            columns: table => new
            {
                Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                EventId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantFeedbacks", x => x.Identifier);
            });

        migrationBuilder.CreateTable(
            name: "ParticipantVotes",
            columns: table => new
            {
                Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                CandidateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantVotes", x => x.Identifier);
            });

        migrationBuilder.CreateTable(
            name: "ExpertGrades",
            columns: table => new
            {
                FeedbackId = table.Column<Guid>(type: "TEXT", nullable: false),
                CriterionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                GradeValue = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExpertGrades", x => new { x.FeedbackId, x.CriterionId });
                table.ForeignKey(
                    name: "FK_ExpertGrades_ExpertFeedbacks_FeedbackId",
                    column: x => x.FeedbackId,
                    principalTable: "ExpertFeedbacks",
                    principalColumn: "Identifier",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ParticipantGrades",
            columns: table => new
            {
                FeedbackId = table.Column<Guid>(type: "TEXT", nullable: false),
                CriterionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                GradeValue = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantGrades", x => new { x.FeedbackId, x.CriterionId });
                table.ForeignKey(
                    name: "FK_ParticipantGrades_ParticipantFeedbacks_FeedbackId",
                    column: x => x.FeedbackId,
                    principalTable: "ParticipantFeedbacks",
                    principalColumn: "Identifier",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <summary>
    /// Откатывает ранее применённую к базе данных миграцию.
    /// </summary>
    /// <param name="migrationBuilder">Конструктор для конфигурирования миграции.</param>
    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AttendanceMarks");

        migrationBuilder.DropTable(
            name: "ExpertGrades");

        migrationBuilder.DropTable(
            name: "ParticipantGrades");

        migrationBuilder.DropTable(
            name: "ParticipantVotes");

        migrationBuilder.DropTable(
            name: "ExpertFeedbacks");

        migrationBuilder.DropTable(
            name: "ParticipantFeedbacks");
    }
}
