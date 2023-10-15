using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Database.Entities;
using ConventionGradingSystem.Models.ExpertGrade;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

public class ExpertGradeModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public ExpertGradeModel(
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

    public void OnGet(int contestId, int eventId)
    {
        var contest = _configuration.Contests.FirstOrDefault(item => item.Identifier == contestId);
        if (contest == null)
        {
            return;
        }

        ViewModel = ViewModel with
        {
            ContestName = contest.Name,
            Criterions = contest.ExpertCriterions
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeCriterion(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    Description: item.Description,
                    MinimalGrade: item.MinimalGrade,
                    MaximalGrade: item.MaximalGrade))
                .ToList()
        };

        var contestEvent = contest.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (contestEvent == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventName = contestEvent.Name };
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(contestId, eventId))
            ? FormState.PreviouslyGraded
            : FormState.NotGraded;
    }

    public async Task OnPostAsync(int contestId, int eventId)
    {
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(contestId, eventId))
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

        foreach (var item in FormModel.Grades)
        {
            _databaseContext.ExpertGrades.Add(new ExpertGrade
            {
                ContestId = contestId,
                EventId = eventId,
                CriterionId = item.CriterionId,
                GradeValue = item.GradeValue
            });
        }

        if (!string.IsNullOrWhiteSpace(FormModel.Note))
        {
            _databaseContext.ExpertNotes.Add(new ExpertNote
            {
                ContestId = contestId,
                EventId = eventId,
                Note = FormModel.Note
            });
        }

        await _databaseContext.SaveChangesAsync();

        Response.Cookies.Append(
            key: GetCookieName(contestId, eventId),
            value: "Мероприятие оценено",
            options: new CookieOptions
            {
                Expires = DateTime.Now.AddHours(4)
            });
    }

    private static string GetCookieName(int contestId, int eventId) =>
        $"ExpertGrade-{contestId}-{eventId}";
}
