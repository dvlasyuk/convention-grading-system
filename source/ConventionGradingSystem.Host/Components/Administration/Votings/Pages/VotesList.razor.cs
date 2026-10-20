using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotesList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<ApplicationConfiguration> _configuration;

    private VotingCandidate? _votingCandidate;
    private IEnumerable<ParticipantVote> _participantVotes = [];
    private string? _searchFilter;

    [Parameter]
    public string? CandidateId { get; set; }

    public VotesList(
        DatabaseContext databaseContext,
        IOptions<ApplicationConfiguration> configuration)
    {
        _databaseContext = databaseContext;
        _configuration = configuration;
    }

    protected override async Task OnInitializedAsync()
    {
        var candidate = _configuration.Value.Votings
            .SelectMany(item => item.Candidates)
            .FirstOrDefault(item => item.Identifier == CandidateId);

        if (candidate == null)
        {
            return;
        }

        var savedVotes = await _databaseContext.ParticipantVotes
            .Where(item => item.CandidateId == CandidateId)
            .ToListAsync();

        var voting = _configuration.Value.Votings.First(item => item.Candidates.Contains(candidate));
        var brigades = _configuration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _configuration.Value.Teams.ToDictionary(team => team.Identifier);

        _votingCandidate = new VotingCandidate(
            CandidateName: candidate.Name,
            VotingName: voting.Name,
            Brigades: [.. candidate.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            VotesQuantity: savedVotes.Count);

        var participants = _configuration.Value.Participants.ToDictionary(item => item.Identifier);
        _participantVotes = [.. savedVotes
            .OrderBy(vote => vote.Identifier)
            .Select((vote, index) => new ParticipantVote(
                Number: index + 1,
                ParticipantName: participants[vote.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[vote.ParticipantId].Brigade)
                    ? brigades[participants[vote.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[vote.ParticipantId].Team].Name,
                Note: vote.Note,
                ReceivedAt: vote.ReceivedAt.ToString(CultureInfo.CurrentCulture)))];
    }

    private bool FilterRecords(ParticipantVote vote)
    {
        return string.IsNullOrWhiteSpace(_searchFilter) ||
            vote.ParticipantName.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            vote.ParticipantTeam.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
            (vote.ParticipantBrigade?.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (vote.Note?.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(ParticipantVote vote) => vote.ParticipantName;
    private static object SelectBrigadeField(ParticipantVote vote) => vote.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(ParticipantVote vote) => vote.ParticipantTeam;

    private sealed record VotingCandidate(
        string CandidateName,
        string VotingName,
        IReadOnlyCollection<string> Brigades,
        int VotesQuantity);

    private sealed record ParticipantVote(
        int Number,
        string ParticipantName,
        string? ParticipantBrigade,
        string ParticipantTeam,
        string? Note,
        string ReceivedAt);
}
