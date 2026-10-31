using ClosedXML.Excel;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Services.Export.Votings;

public class CandidatesListExporter : BaseExporter
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public CandidatesListExporter(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    public async Task<byte[]> ExportToExcel(string votingId)
    {
        var candidates = await LoadData(votingId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Кандидаты голосования");

        var rowNumber = 1;
        var columnNumber = 1;

        FillCell(worksheet, rowNumber, ref columnNumber, "#");
        FillCell(worksheet, rowNumber, ref columnNumber, "Название/имя участника");
        FillCell(worksheet, rowNumber, ref columnNumber, "Связанные отряды");
        FillCell(worksheet, rowNumber, ref columnNumber, "Связанные команды");
        FillCell(worksheet, rowNumber, ref columnNumber, "Количество голосов");

        rowNumber++;

        foreach (var candidate in candidates)
        {
            columnNumber = 1;

            FillCell(worksheet, rowNumber, ref columnNumber, candidate.Number);
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.CandidateName);
            FillCell(worksheet, rowNumber, ref columnNumber, string.Join(Environment.NewLine, candidate.Brigades));
            FillCell(worksheet, rowNumber, ref columnNumber, string.Join(Environment.NewLine, candidate.Teams));
            FillCell(worksheet, rowNumber, ref columnNumber, candidate.VotesQuantity);

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

    private async Task<CandidateData[]> LoadData(string votingId)
    {
        var voting = _dataConfiguration.Value.Votings.FirstOrDefault(item => item.Identifier == votingId)
            ?? throw new InvalidOperationException($"Voting {votingId} is not found");

        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var candidateIds = voting.Candidates
            .Select(item => item.Identifier)
            .ToList();

        var savedVotes = await databaseContext.ParticipantVotes
            .Where(item => candidateIds.Contains(item.CandidateId))
            .ToArrayAsync();

        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var candidatesData = voting.Candidates
            .OrderBy(candidate => candidate.Identifier)
            .Select((candidate, index) => new CandidateData(
                Number: index + 1,
                CandidateName: candidate.Name,
                Brigades: [.. candidate.Brigades
                    .Select(identifier => brigades[identifier])
                    .Select(brigade => brigade.Name)],
                Teams: [.. candidate.Teams
                    .Select(identifier => teams[identifier])
                    .Select(team => team.Name)],
                VotesQuantity: savedVotes
                    .Count(vote => vote.CandidateId == candidate.Identifier)))
            .ToArray();

        return candidatesData;
    }

    private sealed record CandidateData(
        int Number,
        string CandidateName,
        string[] Brigades,
        string[] Teams,
        int VotesQuantity);
}
