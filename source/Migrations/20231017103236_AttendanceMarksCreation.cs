using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class AttendanceMarksCreation : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ParticipationMarks");

        migrationBuilder.DropTable(
            name: "SpecialMarks");

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
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AttendanceMarks");

        migrationBuilder.CreateTable(
            name: "ParticipationMarks",
            columns: table => new
            {
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                EventId = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipationMarks", x => new { x.ParticipantId, x.EventId });
            });

        migrationBuilder.CreateTable(
            name: "SpecialMarks",
            columns: table => new
            {
                ParticipantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                EventId = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SpecialMarks", x => new { x.ParticipantId, x.EventId });
            });
    }
}
