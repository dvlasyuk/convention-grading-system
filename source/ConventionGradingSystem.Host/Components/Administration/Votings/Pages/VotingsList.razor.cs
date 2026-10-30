using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Votings.Components;
using ConventionGradingSystem.Host.Components.Administration.Votings.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class VotingsList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly IDialogService _dialogService;

    public VotingsList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<VotingsListModel>> LoadContent()
    {
        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var savedVotes = await databaseContext.ParticipantVotes.ToArrayAsync();

        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewVotings = _dataConfiguration.Value.Votings
            .OrderBy(voting => voting.Identifier)
            .Select((voting, index) => new VotingsListModel.VotingModel(
                Number: index + 1,
                Identifier: voting.Identifier,
                Name: voting.Name,
                BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
                TeamFriendlyVoting: voting.TeamFriendlyVoting,
                VotesQuantity: voting.VotesQuantity,
                BannedTeams: [.. voting.BannedTeams
                    .Select(identifier => teams[identifier])
                    .Select(team => team.Name)],
                CandidatesQuantity: voting.Candidates.Count,
                TotalVotesQuantity: voting.Candidates.Sum(candidate =>
                    savedVotes.Count(vote => vote.CandidateId == candidate.Identifier))))
            .ToArray();

        return new VotingsListModel(Votings: viewVotings, SearchFilter: null);
    }

    private void OpenVotingCard(VotingsListModel.VotingModel voting)
    {
        var dialogViewModel = new VotingCardModel(
            VotingName: voting.Name,
            BrigadeFriendlyVoting: voting.BrigadeFriendlyVoting,
            TeamFriendlyVoting: voting.TeamFriendlyVoting,
            VotesQuantity: voting.VotesQuantity,
            BannedTeams: voting.BannedTeams);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<VotingCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<VotingCard>(dialogParameters, dialogOptions);
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
