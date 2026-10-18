using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class CandidatesList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private AudienceVoting? _audienceVoting;
    private IEnumerable<VotingCandidate> _votingCandidates = [];
    private string? _searchFilter;

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

        _audienceVoting = new AudienceVoting(
            Name: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            CandidatesQuantity: voting.Candidates.Count,
            TotalVotesQuantity: savedVotes.Count);

        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _configuration.Value.Teams.ToDictionary(team => team.Identifier);

        _votingCandidates = [.. voting.Candidates
            .OrderBy(candidate => candidate.Identifier)
            .Select((candidate, index) => new VotingCandidate(
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

    private bool FilterRecords(VotingCandidate voting)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            voting.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Brigades.Any(brigade => brigade.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase)) ||
            voting.Identifier.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(VotingCandidate voting) => voting.Name;
    private static object SelectQuantityField(VotingCandidate voting) => voting.VotesQuantity;

    private sealed record AudienceVoting(
        string Name,
        bool BrigadeFriendlyVoting,
        bool TeamFriendlyVoting,
        int VotesQuantity,
        int CandidatesQuantity,
        int TotalVotesQuantity);

    private sealed record VotingCandidate(
        int Number,
        string Identifier,
        string Name,
        IReadOnlyCollection<string> Brigades,
        IReadOnlyCollection<string> Teams,
        int VotesQuantity,
        string VotesListLink);
}
