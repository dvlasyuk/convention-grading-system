using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ConventionGradingSystem.Migrations;

public partial class ConfigurationIdentifiersTypeChange : Migration
{
    protected override void Up([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "SpecialMarks",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "SpecialMarks",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "ParticipationMarks",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "ParticipationMarks",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "ParticipantNotes",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "ParticipantNotes",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "ParticipantGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "CriterionId",
            table: "ParticipantGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "ParticipantGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "ExpertNotes",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "ExpertNotes",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "EventId",
            table: "ExpertGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "CriterionId",
            table: "ExpertGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");

        migrationBuilder.AlterColumn<string>(
            name: "ContestId",
            table: "ExpertGrades",
            type: "TEXT",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "INTEGER");
    }

    protected override void Down([NotNull] MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "SpecialMarks",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "SpecialMarks",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "ParticipationMarks",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "ParticipationMarks",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "ParticipantNotes",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "ParticipantNotes",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "ParticipantGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "CriterionId",
            table: "ParticipantGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "ParticipantGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "ExpertNotes",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "ExpertNotes",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "EventId",
            table: "ExpertGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "CriterionId",
            table: "ExpertGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");

        migrationBuilder.AlterColumn<int>(
            name: "ContestId",
            table: "ExpertGrades",
            type: "INTEGER",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "TEXT");
    }
}
