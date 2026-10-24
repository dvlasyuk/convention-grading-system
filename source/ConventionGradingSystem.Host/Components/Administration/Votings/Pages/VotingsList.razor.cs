using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotingsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    private readonly VotingsListModel _viewModel = new();

    public VotingsList(
        DatabaseContext databaseContext,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContext = databaseContext;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task OnInitializedAsync()
    {
        var savedVotes = await _databaseContext.ParticipantVotes.ToListAsync();
        _viewModel.Votings = [.. _dataConfiguration.Value.Votings
            .OrderBy(voting => voting.Identifier)
            .Select((voting, index) => new VotingsListModel.VotingModel(
                Number: index + 1,
                Identifier: voting.Identifier,
                Name: voting.Name,
                CandidatesQuantity: voting.Candidates.Count,
                TotalVotesQuantity: voting.Candidates.Sum(candidate =>
                    savedVotes.Count(vote => vote.CandidateId == candidate.Identifier)),
                CandidatesListLink: $"/administration/votings/{voting.Identifier}/candidates",
                ParticipantVotingFormLink: $"/grading/votings/{voting.Identifier}/participant-voting"))];
    }

    private bool FilterRecords(VotingsListModel.VotingModel voting)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            voting.Name.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Identifier.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(VotingsListModel.VotingModel voting) => voting.Name;
    private static object SelectCandidatesField(VotingsListModel.VotingModel voting) => voting.CandidatesQuantity;
    private static object SelectVotesField(VotingsListModel.VotingModel voting) => voting.TotalVotesQuantity;
}
