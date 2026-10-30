using System.Diagnostics.CodeAnalysis;

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
public partial class CandidatesList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    [Parameter]
    public string? VotingId { get; set; }

    public CandidatesList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<CandidatesListModel>> LoadContent()
    {
        var voting = _dataConfiguration.Value.Votings.FirstOrDefault(item => item.Identifier == VotingId);
        if (voting == null)
        {
            return Shared.Models.LoadingError.VotingNotFound;
        }

        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewVoting = new CandidatesListModel.VotingModel(
            Name: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            BannedTeams: [.. voting.BannedTeams
                .Select(identifier => teams[identifier])
                .Select(team => team.Name)]);

        using var databaseContext = _databaseContextFactory.CreateDbContext();

        var candidateIds = voting.Candidates
            .Select(item => item.Identifier)
            .ToList();

        var savedVotes = await databaseContext.ParticipantVotes
            .Where(item => candidateIds.Contains(item.CandidateId))
            .ToArrayAsync();

        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var viewCandidates = voting.Candidates
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
                    .Count(vote => vote.CandidateId == candidate.Identifier)))
            .ToArray();

        return new CandidatesListModel(
            Voting: viewVoting,
            Candidates: viewCandidates,
            SearchFilter: null);
    }

    private void OpenVotingCard()
    {
        var dialogViewModel = new VotingCardModel(
            VotingName: ViewModel.Voting.Name,
            BrigadeFriendlyVoting: ViewModel.Voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: ViewModel.Voting.TeamFriendlyVoting,
            VotesQuantity: ViewModel.Voting.VotesQuantity,
            BannedTeams: ViewModel.Voting.BannedTeams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<VotingCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<VotingCard>(dialogParameters, dialogOptions);
    }

    private void OpenCandidateCard(CandidatesListModel.CandidateModel candidate)
    {
        var dialogViewModel = new CandidateCardModel(
            CandidateName: candidate.Name,
            VotingName: ViewModel.Voting.Name,
            Brigades: candidate.Brigades,
            Teams: candidate.Teams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<CandidateCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<CandidateCard>(dialogParameters, dialogOptions);
    }

    private bool FilterRecords(CandidatesListModel.CandidateModel voting)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            voting.Name.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            voting.Brigades.Any(brigade => brigade.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase)) ||
            voting.Identifier.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static object SelectNameField(CandidatesListModel.CandidateModel voting) => voting.Name;
    private static object SelectQuantityField(CandidatesListModel.CandidateModel voting) => voting.VotesQuantity;
}
