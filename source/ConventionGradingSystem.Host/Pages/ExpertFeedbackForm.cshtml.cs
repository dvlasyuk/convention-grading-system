using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration;
using ConventionGradingSystem.DataAccess.Database;
using ConventionGradingSystem.DataAccess.Database.Entities;
using ConventionGradingSystem.Host.Models.ExpertFeedbackForm;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Pages;

/// <summary>
/// Модель формы приложения для сбора отзывов экспертов о мероприятии в рамках конкурса мероприятий.
/// </summary>
[Authorize(Roles = "Administrator,Expert")]
public class ExpertFeedbackFormModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="ExpertFeedbackFormModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    /// <param name="databaseContext">Контекст для доступа к базе данных.</param>
    public ExpertFeedbackFormModel(
        [NotNull] IOptions<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Состояние формы.
    /// </summary>
    public FormState FormState { get; private set; } = FormState.NotExisted;

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        EventName: "Неизвестное мероприятие",
        Criterions: new List<GradeCriterion>());

    /// <summary>
    /// Модель данных формы.
    /// </summary>
    [BindProperty]
    public FormModel? FormModel { get; set; }

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
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

        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventId))
            ? FormState.PreviouslyGraded
            : FormState.NotGraded;
    }

    /// <summary>
    /// Обрабатывает POST-запрос к странице.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    public async Task OnPostAsync(string eventId)
    {
        var contestEvent = _configuration.Contests
            .SelectMany(item => item.Events)
            .FirstOrDefault(item => item.Identifier == eventId);

        if (contestEvent == null)
        {
            return;
        }

        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventId))
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

        var feedback = new ExpertFeedback
        {
            EventId = eventId,
            ExpertId = null!,
            Note = !string.IsNullOrWhiteSpace(FormModel.Note)
                ? FormModel.Note.Trim()
                : null
        };

        foreach (var item in FormModel.Grades)
        {
            feedback.Grades.Add(new ExpertGrade
            {
                CriterionId = item.CriterionId,
                GradeValue = item.GradeValue
            });
        }

        _databaseContext.ExpertFeedbacks.Add(feedback);
        await _databaseContext.SaveChangesAsync();

        Response.Cookies.Append(
            key: GetCookieName(eventId),
            value: "Мероприятие оценено",
            options: new CookieOptions
            {
                Expires = DateTime.Now.AddHours(4)
            });
    }

    private static string GetCookieName(string eventId) =>
        $"ExpertFeedback-{eventId}";
}
