using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class ContestsAndCriterionsRenaming : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "SpecialMarks",
            newName: "ContestId");

        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "ParticipationMarks",
            newName: "ContestId");

        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "ParticipantNotes",
            newName: "ContestId");

        migrationBuilder.RenameColumn(
            name: "GradeTypeId",
            table: "ParticipantGrades",
            newName: "CriterionId");

        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "ParticipantGrades",
            newName: "ContestId");

        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "ExpertNotes",
            newName: "ContestId");

        migrationBuilder.RenameColumn(
            name: "GradeTypeId",
            table: "ExpertGrades",
            newName: "CriterionId");

        migrationBuilder.RenameColumn(
            name: "EventTypeId",
            table: "ExpertGrades",
            newName: "ContestId");
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "SpecialMarks",
            newName: "EventTypeId");

        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "ParticipationMarks",
            newName: "EventTypeId");

        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "ParticipantNotes",
            newName: "EventTypeId");

        migrationBuilder.RenameColumn(
            name: "CriterionId",
            table: "ParticipantGrades",
            newName: "GradeTypeId");

        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "ParticipantGrades",
            newName: "EventTypeId");

        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "ExpertNotes",
            newName: "EventTypeId");

        migrationBuilder.RenameColumn(
            name: "CriterionId",
            table: "ExpertGrades",
            newName: "GradeTypeId");

        migrationBuilder.RenameColumn(
            name: "ContestId",
            table: "ExpertGrades",
            newName: "EventTypeId");
    }
}
