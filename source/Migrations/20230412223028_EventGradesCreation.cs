using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class EventGradesCreation : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ExpertGrades",
            columns: table => new
            {
                Identifier = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                EventId = table.Column<int>(type: "INTEGER", nullable: false),
                GradeTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                GradeValue = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExpertGrades", x => x.Identifier);
            });

        migrationBuilder.CreateTable(
            name: "ParticipantGrades",
            columns: table => new
            {
                Identifier = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                EventTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                EventId = table.Column<int>(type: "INTEGER", nullable: false),
                GradeTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                GradeValue = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantGrades", x => x.Identifier);
            });
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExpertGrades");

        migrationBuilder.DropTable(
            name: "ParticipantGrades");
    }
}
