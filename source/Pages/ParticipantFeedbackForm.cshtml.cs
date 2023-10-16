using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Database.Entities;
using ConventionGradingSystem.Models.ParticipantFeedbackForm;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

public class ParticipantFeedbackFormModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public ParticipantFeedbackFormModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public FormState FormState { get; private set; } = FormState.NotExisted;
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        EventName: "Неизвестное мероприятие",
        Criterions: new List<GradeCriterion>());

    [BindProperty]
    public FormModel? FormModel { get; set; }

    public void OnGet(string eventId)
    {
        var contestEvent = _configuration.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == eventId);

        if (contestEvent == null)
        {
            return;
        }

        var contest = _configuration.Contests.First(item => item.Events.Contains(contestEvent));
        ViewModel = ViewModel with
        {
            ContestName = contest.Name,
            EventName = contestEvent.Name,
            Criterions = contest.ParticipantCriterions
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeCriterion(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    Description: item.Description,
                    MinimalGrade: item.MinimalGrade,
                    MaximalGrade: item.MaximalGrade))
                .ToList()
        };

        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(contest.Identifier))
            ? FormState.PreviouslyGraded
            : FormState.NotGraded;
    }

    public async Task OnPostAsync(string eventId)
    {
        var contest = _configuration.Contests.FirstOrDefault(item => item.Events.Any(item => item.Identifier == eventId));
        if (contest == null)
        {
            return;
        }

        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(contest.Identifier))
            ? FormState.PreviouslyGraded
            : FormState.JustGraded;

        if (FormState == FormState.PreviouslyGraded)
        {
            return;
        }
        if (FormModel == null)
        {
            throw new InvalidOperationException("Модель формы должна быть заполнена при выполнении POST-запроса");
        }

        var feedback = new ParticipantFeedback
        {
            EventId = eventId,
            Note = !string.IsNullOrWhiteSpace(FormModel.Note)
                ? FormModel.Note.Trim()
                : null
        };

        foreach (var item in FormModel.Grades)
        {
            feedback.Grades.Add(new ParticipantGrade
            {
                CriterionId = item.CriterionId,
                GradeValue = item.GradeValue
            });
        }

        _databaseContext.ParticipantFeedbacks.Add(feedback);
        await _databaseContext.SaveChangesAsync();

        Response.Cookies.Append(
            key: GetCookieName(contest.Identifier),
            value: "Конкурс мероприятий оценён",
            options: new CookieOptions
            {
                Expires = DateTime.Now.AddHours(4)
            });
    }

    private static string GetCookieName(string contestId)
    {
        return $"ParticipantGrade-{contestId}";
    }
}
