using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Components;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotesList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    [Parameter]
    public string? CandidateId { get; set; }

    public VotesList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<VotesListModel>> LoadContent()
    {
        var candidate = _dataConfiguration.Value.Votings
            .SelectMany(item => item.Candidates)
            .FirstOrDefault(item => item.Identifier == CandidateId);

        if (candidate == null)
        {
            return Shared.Models.LoadingError.CandidateNotFound;
        }

        var voting = _dataConfiguration.Value.Votings.First(item => item.Candidates.Contains(candidate));
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewCandidate = new VotesListModel.CandidateModel(
            CandidateName: candidate.Name,
            VotingName: voting.Name,
            Brigades: [.. candidate.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            Teams: [.. candidate.Teams
                .Select(identifier => teams[identifier])
                .Select(team => team.Name)]);

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedVotes = await databaseContext.ParticipantVotes
            .Where(item => item.CandidateId == candidate.Identifier)
            .ToArrayAsync();

        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        var viewVotes = savedVotes
            .OrderBy(vote => vote.Identifier)
            .Select((vote, index) => new VotesListModel.VoteModel(
                Number: index + 1,
                ParticipantName: participants[vote.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[vote.ParticipantId].Brigade)
                    ? brigades[participants[vote.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[vote.ParticipantId].Team].Name,
                Note: vote.Note,
                ReceivedAt: vote.ReceivedAt.ToString(CultureInfo.CurrentCulture)))
            .ToArray();

        return new VotesListModel(
            Candidate: viewCandidate,
            Votes: viewVotes,
            SearchFilter: null);
    }

    private void OpenCandidateCard()
    {
        var dialogViewModel = new CandidateCardModel(
            CandidateName: ViewModel.Candidate.CandidateName,
            VotingName: ViewModel.Candidate.VotingName,
            Brigades: ViewModel.Candidate.Brigades,
            Teams: ViewModel.Candidate.Teams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<CandidateCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<CandidateCard>(dialogParameters, dialogOptions);
    }

    private bool FilterRecords(VotesListModel.VoteModel vote)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            vote.ParticipantName.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            vote.ParticipantTeam.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (vote.ParticipantBrigade?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (vote.Note?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(VotesListModel.VoteModel vote) => vote.ParticipantName;
    private static object SelectBrigadeField(VotesListModel.VoteModel vote) => vote.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(VotesListModel.VoteModel vote) => vote.ParticipantTeam;
}
