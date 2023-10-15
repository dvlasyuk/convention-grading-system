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
        EventTypeName: "Неизвестная категория",
        EventName: "Неизвестное мероприятие",
        GradeTypes: new List<GradeType>());

    [BindProperty]
    public FormModel FormModel { get; set; }

    public void OnGet(int eventTypeId, int eventId)
    {
        var eventType = _configuration.EventTypes.FirstOrDefault(item => item.Identifier == eventTypeId);
        if (eventType == null)
        {
            return;
        }

        ViewModel = ViewModel with
        {
            EventTypeName = eventType.Name,
            GradeTypes = eventType.ExpertGrades
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeType(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    Description: item.Description,
                    MinimalGrage: item.MinimalGrage,
                    MaximalGrage: item.MaximalGrage))
                .ToList()
        };

        var @event = eventType.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (@event == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventName = @event.Name };
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId, eventId))
            ? FormState.PreviouslyGraded
            : FormState.NotGraded;
    }

    public async Task OnPostAsync(int eventTypeId, int eventId)
    {
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId, eventId))
            ? FormState.PreviouslyGraded
            : FormState.JustGraded;

        if (FormState == FormState.PreviouslyGraded)
        {
            return;
        }

        foreach (var item in FormModel.Grades)
        {
            _databaseContext.ExpertGrades.Add(new ExpertGrade
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                GradeTypeId = item.GradeTypeId,
                GradeValue = item.GradeValue
            });
        }

        if (!string.IsNullOrWhiteSpace(FormModel.Note))
        {
            _databaseContext.ExpertNotes.Add(new ExpertNote
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                Note = FormModel.Note
            });
        }

        await _databaseContext.SaveChangesAsync();

        Response.Cookies.Append(
            key: GetCookieName(eventTypeId, eventId),
            value: "Мероприятие оценено",
            options: new CookieOptions
            {
                Expires = DateTime.Now.AddHours(4)
            });
    }

    private static string GetCookieName(int eventTypeId, int eventId) =>
        $"ExpertGrade-{eventTypeId}-{eventId}";
}
