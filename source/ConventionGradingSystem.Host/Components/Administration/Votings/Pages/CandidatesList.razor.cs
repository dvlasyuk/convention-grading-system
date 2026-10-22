using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class CandidatesList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private readonly CandidatesListModel _viewModel = new();

    [Parameter]
    public string? VotingId { get; set; }

    public CandidatesList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var voting = _configuration.Value.Votings.FirstOrDefault(item => item.Identifier == VotingId);
        if (voting == null)
        {
            return;
        }

        var candidateIds = voting.Candidates
            .Select(item => item.Identifier)
            .ToList();

        var savedVotes = await _databaseContext.ParticipantVotes
            .Where(item => candidateIds.Contains(item.CandidateId))
            .ToListAsync();

        _viewModel.Voting = new CandidatesListModel.VotingModel(
            Name: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            CandidatesQuantity: voting.Candidates.Count,
            TotalVotesQuantity: savedVotes.Count);

        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _configuration.Value.Teams.ToDictionary(team => team.Identifier);

        _viewModel.Candidates = [.. voting.Candidates
            .OrderBy(candidate => candidate.Identifier)
            .Select((candidate, index) => new CandidatesListModel.CandidateModel(
                Number: index + 1,
                Identifier: candidate.Identifier,
                Name: candidate.Name,
                Brigades: [.. candidate.Brigades
                    .Select(identifier => brigades[identifier])
                    .Select(brigade => brigade.Name)],
                Teams: [.. candidate.Teams
                    .Select(identifier => teams[identifier])
                    .Select(team => team.Name)],
                VotesQuantity: savedVotes
                    .Count(vote => vote.CandidateId == candidate.Identifier),
                VotesListLink: $"/administration/candidates/{candidate.Identifier}/votes"))];
    }

    private bool FilterRecords(CandidatesListModel.CandidateModel voting)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            voting.Name.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Brigades.Any(brigade => brigade.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase)) ||
            voting.Identifier.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(CandidatesListModel.CandidateModel voting) => voting.Name;
    private static object SelectQuantityField(CandidatesListModel.CandidateModel voting) => voting.VotesQuantity;
}
