using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Database.Entities;
using ConventionGradingSystem.Models.ParticipantGrade;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

public class ParticipantGradeModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    public ParticipantGradeModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    public GradeState GradeState { get; set; } = GradeState.NotExisted;
    public Event Event { get; set; } = new Event
    {
        EventTypeName = "Неизвестная категория",
        EventName = "Неизвестное мероприятие",
        GradeTypes = new List<GradeType>()
    };

    [BindProperty]
    public List<Grade> Grades { get; set; }

    [BindProperty]
    public string Note { get; set; }

    public void OnGet(int eventTypeId, int eventId)
    {
        var eventType = _configuration.EventTypes.FirstOrDefault(item => item.Identifier == eventTypeId);
        if (eventType == null)
        {
            return;
        }

        Event.EventTypeName = eventType.Name;
        Event.GradeTypes = eventType.ParticipantGrades
            .OrderBy(item => item.Identifier)
            .Select(item => new GradeType
            {
                Identifier = item.Identifier,
                Name = item.Name,
                Description = item.Description,
                MinimalGrage = item.MinimalGrage,
                MaximalGrage = item.MaximalGrage
            })
            .ToList();

        var @event = eventType.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (@event == null)
        {
            return;
        }

        Event.EventName = @event.Name;
        GradeState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId))
            ? GradeState.PreviouslyGraded
            : GradeState.NotGraded;
    }

    public async Task OnPostAsync(int eventTypeId, int eventId)
    {
        GradeState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId))
            ? GradeState.PreviouslyGraded
            : GradeState.JustGraded;

        if (GradeState == GradeState.PreviouslyGraded)
        {
            return;
        }

        foreach (var item in Grades)
        {
            _databaseContext.ParticipantGrades.Add(new ParticipantGrade
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                GradeTypeId = item.GradeTypeId,
                GradeValue = item.GradeValue
            });
        }

        if (!string.IsNullOrWhiteSpace(Note))
        {
            _databaseContext.ParticipantNotes.Add(new ParticipantNote
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                Note = Note
            });
        }

        await _databaseContext.SaveChangesAsync();

        Response.Cookies.Append(
            key: GetCookieName(eventTypeId),
            value: "Категория мероприятий оценена",
            options: new CookieOptions
            {
                Expires = DateTime.Now.AddHours(4)
            });
    }

    private static string GetCookieName(int eventTypeId)
    {
        return $"ParticipantGrade-{eventTypeId}";
    }
}
