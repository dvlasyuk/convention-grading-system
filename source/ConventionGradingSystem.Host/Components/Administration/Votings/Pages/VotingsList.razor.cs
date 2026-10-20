using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotingsList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private IEnumerable<AudienceVoting> _audienceVotings = [];
    private string? _searchFilter;

    public VotingsList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var savedVotes = await _databaseContext.ParticipantVotes.ToListAsync();
        _audienceVotings = [.. _configuration.Value.Votings
            .OrderBy(voting => voting.Identifier)
            .Select((voting, index) => new AudienceVoting(
                Number: index + 1,
                Identifier: voting.Identifier,
                Name: voting.Name,
                CandidatesQuantity: voting.Candidates.Count,
                TotalVotesQuantity: voting.Candidates.Sum(candidate =>
                    savedVotes.Count(vote => vote.CandidateId == candidate.Identifier))))];
    }

    private bool FilterRecords(AudienceVoting voting)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            voting.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Identifier.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(AudienceVoting voting) => voting.Name;
    private static object SelectCandidatesField(AudienceVoting voting) => voting.CandidatesQuantity;
    private static object SelectVotesField(AudienceVoting voting) => voting.TotalVotesQuantity;

    private sealed record AudienceVoting(
        int Number,
        string Identifier,
        string Name,
        int CandidatesQuantity,
        int TotalVotesQuantity);
}
