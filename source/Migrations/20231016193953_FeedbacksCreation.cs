using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class FeedbacksCreation : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExpertNotes");

        migrationBuilder.DropTable(
            name: "ParticipantNotes");

        migrationBuilder.RenameColumn(
            name: "EventId",
            table: "ParticipantGrades",
            newName: "FeedbackId");

        migrationBuilder.RenameColumn(
            name: "EventId",
            table: "ExpertGrades",
            newName: "FeedbackId");

        migrationBuilder.AlterColumn<Guid>(
            name: "Identifier",
            table: "ParticipantGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<Guid>(
            name: "Identifier",
            table: "ExpertGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER")
            .OldAnnotation("Sqlite:Autoincrement", true);

        migrationBuilder.CreateTable(
            name: "ExpertFeedbacks",
            columns: table => new
            {
                Identifier = table.Column<Guid>(type: "TEXT", nullable: false),
                EventId = table.Column<string>(type: "TEXT", nullable: false),
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
                EventId = table.Column<string>(type: "TEXT", nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantFeedbacks", x => x.Identifier);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantGrades_FeedbackId",
            table: "ParticipantGrades",
            column: "FeedbackId");

        migrationBuilder.CreateIndex(
            name: "IX_ExpertGrades_FeedbackId",
            table: "ExpertGrades",
            column: "FeedbackId");

        migrationBuilder.AddForeignKey(
            name: "FK_ExpertGrades_ExpertFeedbacks_FeedbackId",
            table: "ExpertGrades",
            column: "FeedbackId",
            principalTable: "ExpertFeedbacks",
            principalColumn: "Identifier",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ParticipantGrades_ParticipantFeedbacks_FeedbackId",
            table: "ParticipantGrades",
            column: "FeedbackId",
            principalTable: "ParticipantFeedbacks",
            principalColumn: "Identifier",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ExpertGrades_ExpertFeedbacks_FeedbackId",
            table: "ExpertGrades");

        migrationBuilder.DropForeignKey(
            name: "FK_ParticipantGrades_ParticipantFeedbacks_FeedbackId",
            table: "ParticipantGrades");

        migrationBuilder.DropTable(
            name: "ExpertFeedbacks");

        migrationBuilder.DropTable(
            name: "ParticipantFeedbacks");

        migrationBuilder.DropIndex(
            name: "IX_ParticipantGrades_FeedbackId",
            table: "ParticipantGrades");

        migrationBuilder.DropIndex(
            name: "IX_ExpertGrades_FeedbackId",
            table: "ExpertGrades");

        migrationBuilder.RenameColumn(
            name: "FeedbackId",
            table: "ParticipantGrades",
            newName: "EventId");

        migrationBuilder.RenameColumn(
            name: "FeedbackId",
            table: "ExpertGrades",
            newName: "EventId");

        migrationBuilder.AlterColumn<int>(
            name: "Identifier",
            table: "ParticipantGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "TEXT")
            .Annotation("Sqlite:Autoincrement", true);

        migrationBuilder.AlterColumn<int>(
            name: "Identifier",
            table: "ExpertGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "TEXT")
            .Annotation("Sqlite:Autoincrement", true);

        migrationBuilder.CreateTable(
            name: "ExpertNotes",
            columns: table => new
            {
                Identifier = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                EventId = table.Column<string>(type: "TEXT", nullable: false),
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
                EventId = table.Column<string>(type: "TEXT", nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantNotes", x => x.Identifier);
            });
    }
}
