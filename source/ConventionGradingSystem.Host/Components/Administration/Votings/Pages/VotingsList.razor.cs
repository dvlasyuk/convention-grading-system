using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotingsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    public VotingsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task<ContentState<VotingsListModel>> LoadContent()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedVotes = await databaseContext.ParticipantVotes.ToListAsync();

        var viewVotings = _dataConfiguration.Value.Votings
            .OrderBy(voting => voting.Identifier)
            .Select((voting, index) => new VotingsListModel.VotingModel(
                Number: index + 1,
                Identifier: voting.Identifier,
                Name: voting.Name,
                CandidatesQuantity: voting.Candidates.Count,
                TotalVotesQuantity: voting.Candidates.Sum(candidate =>
                    savedVotes.Count(vote => vote.CandidateId == candidate.Identifier))))
            .ToArray();

        return new VotingsListModel(Votings: viewVotings, SearchFilter: null);
    }

    private bool FilterRecords(VotingsListModel.VotingModel voting)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            voting.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Identifier.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(VotingsListModel.VotingModel voting) => voting.Name;
    private static object SelectCandidatesField(VotingsListModel.VotingModel voting) => voting.CandidatesQuantity;
    private static object SelectVotesField(VotingsListModel.VotingModel voting) => voting.TotalVotesQuantity;
}
