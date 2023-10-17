using System.Data;
using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Host.Configuration;
using ConventionGradingSystem.Host.Database;
using ConventionGradingSystem.Host.Models.ExpertFeedbacksPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Host.Pages;

/// <summary>
/// Модель страницы приложения со списком отзывов экспертов о мероприятии в рамках конкурса мероприятий.
/// </summary>
[Authorize(Roles = "Administrator")]
public class ExpertFeedbacksPageModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="ExpertFeedbacksPageModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    /// <param name="databaseContext">Контекст для доступа к базе данных.</param>
    public ExpertFeedbacksPageModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    public ViewModel ViewModel { get; set; } = new ViewModel(
        ContestName: "Неизвестный конкурс",
        EventName: "Неизвестное мероприятие",
        Criterions: new List<GradeCriterion>(),
        Feedbacks: new List<Feedback>());

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    public async Task OnGetAsync(string eventId)
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
                    Name: item.Name))
                .ToList()
        };

        var feedbacks = await _databaseContext.ExpertFeedbacks
            .Where(item => item.EventId == eventId)
            .Include(item => item.Grades)
            .ToListAsync();

        ViewModel = ViewModel with
        {
            Feedbacks = feedbacks
                .OrderBy(item => item.Identifier)
                .Select(item => new Feedback(
                    Grades: item.Grades.ToDictionary(
                        item => item.CriterionId,
                        item => item.GradeValue),
                    Note: item.Note))
                .ToList()
        };
    }
}
