using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotesList
{
    private readonly DatabaseContext _databaseContext;
    private readonly IOptions<DataConfiguration> _dataConfiguration;

    private readonly VotesListModel _viewModel = new();

    [Parameter]
    public string? CandidateId { get; set; }

    public VotesList(
        DatabaseContext databaseContext,
        IOptions<DataConfiguration> dataConfiguration)
    {
        _databaseContext = databaseContext;
        _dataConfiguration = dataConfiguration;
    }

    protected override async Task OnInitializedAsync()
    {
        var candidate = _dataConfiguration.Value.Votings
            .SelectMany(item => item.Candidates)
            .FirstOrDefault(item => item.Identifier == CandidateId);

        if (candidate == null)
        {
            return;
        }

        var savedVotes = await _databaseContext.ParticipantVotes
            .Where(item => item.CandidateId == CandidateId)
            .ToListAsync();

        var voting = _dataConfiguration.Value.Votings.First(item => item.Candidates.Contains(candidate));
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);

        _viewModel.Candidate = new VotesListModel.CandidateModel(
            CandidateName: candidate.Name,
            VotingName: voting.Name,
            Brigades: [.. candidate.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            VotesQuantity: savedVotes.Count);

        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        _viewModel.Votes = [.. savedVotes
            .OrderBy(vote => vote.Identifier)
            .Select((vote, index) => new VotesListModel.VoteModel(
                Number: index + 1,
                ParticipantName: participants[vote.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[vote.ParticipantId].Brigade)
                    ? brigades[participants[vote.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[vote.ParticipantId].Team].Name,
                Note: vote.Note,
                ReceivedAt: vote.ReceivedAt.ToString(new CultureInfo("ru-RU"))))];
    }

    private bool FilterRecords(VotesListModel.VoteModel vote)
    {
        return string.IsNullOrWhiteSpace(_viewModel.SearchFilter) ||
            vote.ParticipantName.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            vote.ParticipantTeam.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (vote.ParticipantBrigade?.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (vote.Note?.Contains(_viewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(VotesListModel.VoteModel vote) => vote.ParticipantName;
    private static object SelectBrigadeField(VotesListModel.VoteModel vote) => vote.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(VotesListModel.VoteModel vote) => vote.ParticipantTeam;
}
