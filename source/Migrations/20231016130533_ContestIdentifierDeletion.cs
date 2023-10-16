using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class ContestIdentifierDeletion : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_SpecialMarks",
            table: "SpecialMarks");

        migrationBuilder.DropPrimaryKey(
            name: "PK_ParticipationMarks",
            table: "ParticipationMarks");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "SpecialMarks");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "ParticipationMarks");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "ParticipantNotes");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "ParticipantGrades");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "ExpertNotes");

        migrationBuilder.DropColumn(
            name: "ContestId",
            table: "ExpertGrades");

        migrationBuilder.AddPrimaryKey(
            name: "PK_SpecialMarks",
            table: "SpecialMarks",
            columns: new[] { "ParticipantId", "EventId" });

        migrationBuilder.AddPrimaryKey(
            name: "PK_ParticipationMarks",
            table: "ParticipationMarks",
            columns: new[] { "ParticipantId", "EventId" });
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_SpecialMarks",
            table: "SpecialMarks");

        migrationBuilder.DropPrimaryKey(
            name: "PK_ParticipationMarks",
            table: "ParticipationMarks");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "SpecialMarks",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "ParticipationMarks",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "ParticipantNotes",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "ParticipantGrades",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "ExpertNotes",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContestId",
            table: "ExpertGrades",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddPrimaryKey(
            name: "PK_SpecialMarks",
            table: "SpecialMarks",
            columns: new[] { "ParticipantId", "ContestId", "EventId" });

        migrationBuilder.AddPrimaryKey(
            name: "PK_ParticipationMarks",
            table: "ParticipationMarks",
            columns: new[] { "ParticipantId", "ContestId", "EventId" });
    }
}
