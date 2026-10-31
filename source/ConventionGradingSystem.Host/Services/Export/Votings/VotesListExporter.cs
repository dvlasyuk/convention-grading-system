using ClosedXML.Excel;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services.Export.Votings;

public class VotesListExporter : BaseExporter
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public VotesListExporter(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    public async Task<byte[]> ExportToExcel(string candidateId)
    {
        var candidates = await LoadData(candidateId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Голоса за кандидата");

        var rowNumber = 1;
        var columnNumber = 1;

        FillCell(worksheet, rowNumber, ref columnNumber, "#");
        FillCell(worksheet, rowNumber, ref columnNumber, "Имя участника");
        FillCell(worksheet, rowNumber, ref columnNumber, "Отряд участника");
        FillCell(worksheet, rowNumber, ref columnNumber, "Команда участника");
        FillCell(worksheet, rowNumber, ref columnNumber, "Дополнительный комментарий");
        FillCell(worksheet, rowNumber, ref columnNumber, "Время получения");

        rowNumber++;

        foreach (var candidate in candidates)
        {
            columnNumber = 1;

            FillCell(worksheet, rowNumber, ref columnNumber, candidate.Number);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.ParticipantName);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.ParticipantBrigade);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.ParticipantTeam);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.Note);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.ReceivedAt);

            rowNumber++;
        }

        FormatTable(
            worksheet: worksheet,
            startRowNumber: 1,
            startColumnNumber: 1,
            tableRowsCount: rowNumber - 1,
            tableColumnsCount: columnNumber - 1,
            headerRowsCount: 1);

        return ExportWorkbook(workbook);
    }

    private async Task<VoteData[]> LoadData(string candidateId)
    {
        var candidate = _dataConfiguration.Value.Votings
                .SelectMany(item => item.Candidates)
                .FirstOrDefault(item => item.Identifier == candidateId)
            ?? throw new InvalidOperationException($"Candidate {candidateId} is not found");

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedVotes = await databaseContext.ParticipantVotes
            .Where(item => item.CandidateId == candidate.Identifier)
            .ToArrayAsync();

        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var votesData = savedVotes
            .OrderBy(vote => vote.Identifier)
            .Select((vote, index) => new VoteData(
                Number: index + 1,
                ParticipantName: participants[vote.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[vote.ParticipantId].Brigade)
                    ? brigades[participants[vote.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[vote.ParticipantId].Team].Name,
                Note: vote.Note,
                ReceivedAt: vote.ReceivedAt.DateTime))
            .ToArray();

        return votesData;
    }

    private sealed record VoteData(
        int Number,
        string ParticipantName,
        string? ParticipantBrigade,
        string ParticipantTeam,
        string? Note,
        DateTime ReceivedAt);
}
