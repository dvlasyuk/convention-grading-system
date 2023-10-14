using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace EventGradingSystem.Migrations
{
    public partial class EventNotesCreate : Migration
    {
        protected override void Up([NotNull] MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpertNotes",
                columns: table => new
                {
                    Identifier = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertNotes", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantNotes",
                columns: table => new
                {
                    Identifier = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantNotes", x => x.Identifier);
                });
        }

        protected override void Down([NotNull] MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpertNotes");

            migrationBuilder.DropTable(
                name: "ParticipantNotes");
        }
    }
}
