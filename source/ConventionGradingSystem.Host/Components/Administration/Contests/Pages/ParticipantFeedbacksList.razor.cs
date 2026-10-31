using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.Host.Components.Administration.Contests.Components;
using ConventionGradingSystem.Host.Components.Administration.Contests.Models;
using ConventionGradingSystem.Host.Components.Shared.Models;
using ConventionGradingSystem.Host.Services;
using ConventionGradingSystem.Host.Services.Export.Contests;

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Pages;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public partial class ParticipantFeedbacksList
{
    private readonly IDbContextFactory<DatabaseContext> _databaseContextFactory;
    private readonly IOptions<DataConfiguration> _dataConfiguration;
    private readonly ParticipantFeedbacksListExporter _participantFeedbacksListExporter;
    private readonly FileDownloader _fileDownloader;
    private readonly IDialogService _dialogService;

    [Parameter]
    public string? EventId { get; set; }

    public ParticipantFeedbacksList(
        IDbContextFactory<DatabaseContext> databaseContextFactory,
        IOptions<DataConfiguration> dataConfiguration,
        ParticipantFeedbacksListExporter participantFeedbacksListExporter,
        FileDownloader fileDownloader,
        IDialogService dialogService)
    {
        _databaseContextFactory = databaseContextFactory;
        _dataConfiguration = dataConfiguration;
        _participantFeedbacksListExporter = participantFeedbacksListExporter;
        _fileDownloader = fileDownloader;
        _dialogService = dialogService;
    }

    protected override async Task<ContentState<ParticipantFeedbacksListModel>> LoadContent()
    {
        var contestEvent = _dataConfiguration.Value.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == EventId);

        if (contestEvent == null)
        {
            return Shared.Models.LoadingError.EventNotFound;
        }

        var contest = _dataConfiguration.Value.Contests.First(item => item.Events.Contains(contestEvent));
        var brigades = _dataConfiguration.Value.Brigades.ToDictionary(brigade => brigade.Identifier);
        var teams = _dataConfiguration.Value.Teams.ToDictionary(team => team.Identifier);
        var viewEvent = new ParticipantFeedbacksListModel.EventModel(
            Identifier: contestEvent.Identifier,
            EventName: contestEvent.Name,
            ContestName: contest.Name,
            ParticipantGrading: contest.ParticipantCriterions.Count > 0,
            ParticipantsRegistered: contestEvent.Participants.Count > 0,
            Brigades: [.. contestEvent.Brigades
                .Select(identifier => brigades[identifier])
                .Select(brigade => brigade.Name)],
            GradeCriterions: [.. contest.ParticipantCriterions
                .Select(criterion => new ParticipantFeedbacksListModel.CriterionModel(
                    Identifier: criterion.Identifier,
                    Name: criterion.Name))]);

        using var databaseContext = _databaseContextFactory.CreateDbContext();
        var feedbacks = await databaseContext.ParticipantFeedbacks
            .Where(item => item.EventId == contestEvent.Identifier)
            .Include(item => item.Grades)
            .ToArrayAsync();

        var participants = _dataConfiguration.Value.Participants.ToDictionary(item => item.Identifier);
        var viewFeedbacks = feedbacks
            .OrderBy(feedback => feedback.Identifier)
            .Select((feedback, index) => new ParticipantFeedbacksListModel.FeedbackModel(
                Number: index + 1,
                ParticipantName: participants[feedback.ParticipantId].Name,
                ParticipantBrigade: !string.IsNullOrWhiteSpace(participants[feedback.ParticipantId].Brigade)
                    ? brigades[participants[feedback.ParticipantId].Brigade!].Name
                    : null,
                ParticipantTeam: teams[participants[feedback.ParticipantId].Team].Name,
                Grades: feedback.Grades.ToDictionary(
                    grade => grade.CriterionId,
                    grade => grade.GradeValue),
                Note: feedback.Note,
                ReceivedAt: feedback.ReceivedAt.ToString(CultureInfo.CurrentCulture)))
            .ToArray();

        return new ParticipantFeedbacksListModel(
            Event: viewEvent,
            Feedbacks: viewFeedbacks,
            SearchFilter: null);
    }

    private void OpenEventCard()
    {
        var dialogViewModel = new EventCardModel(
            EventName: ViewModel.Event.EventName,
            ContestName: ViewModel.Event.ContestName,
            ParticipantsRegistered: ViewModel.Event.ParticipantsRegistered,
            Brigades: ViewModel.Event.Brigades);

        var dialogOptions = new DialogOptions { CloseButton = true };
        var dialogParameters = new DialogParameters<EventCard>
        {
            { dialog => dialog.ViewModel, dialogViewModel }
        };

        _dialogService.ShowAsync<EventCard>(dialogParameters, dialogOptions);
    }

    private async Task ExportDataToExcel()
    {
        var fileName = $"{ViewModel.Event.EventName} - Отзывы участников.xlsx";
        var fileData = await _participantFeedbacksListExporter.ExportToExcel(ViewModel.Event.Identifier);

        await _fileDownloader.DownloadExcelFile(fileName, fileData);
    }

    private bool FilterRecords(ParticipantFeedbacksListModel.FeedbackModel feedback)
    {
        return string.IsNullOrWhiteSpace(ViewModel.SearchFilter) ||
            feedback.ParticipantName.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            feedback.ParticipantTeam.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ||
            (feedback.ParticipantBrigade?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (feedback.Note?.Contains(ViewModel.SearchFilter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static object SelectNameField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantName;
    private static object SelectBrigadeField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantBrigade ?? string.Empty;
    private static object SelectTeamField(ParticipantFeedbacksListModel.FeedbackModel feedback) => feedback.ParticipantTeam;

    private static object SelectGradeField(ParticipantFeedbacksListModel.FeedbackModel feedback, string gradeId) => feedback.Grades[gradeId];
}
