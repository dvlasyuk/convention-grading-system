using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace EventGradingSystem.Migrations
{
    public partial class EventMarksCreate : Migration
    {
        protected override void Up([NotNull] MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParticipationMarks",
                columns: table => new
                {
                    ParticipantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipationMarks", x => new { x.ParticipantId, x.EventTypeId, x.EventId });
                });

            migrationBuilder.CreateTable(
                name: "SpecialMarks",
                columns: table => new
                {
                    ParticipantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialMarks", x => new { x.ParticipantId, x.EventTypeId, x.EventId });
                });
        }

        protected override void Down([NotNull] MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipationMarks");

            migrationBuilder.DropTable(
                name: "SpecialMarks");
        }
    }
}
