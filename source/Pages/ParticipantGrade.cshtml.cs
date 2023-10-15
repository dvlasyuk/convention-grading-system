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

    public FormState FormState { get; private set; } = FormState.NotExisted;
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        EventTypeName: "Неизвестная категория",
        EventName: "Неизвестное мероприятие",
        GradeTypes: new List<GradeType>());

    [BindProperty]
    public FormModel? FormModel { get; set; }

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
            GradeTypes = eventType.ParticipantGrades
                .OrderBy(item => item.Identifier)
                .Select(item => new GradeType(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    Description: item.Description,
                    MinimalGrade: item.MinimalGrade,
                    MaximalGrade: item.MaximalGrade))
                .ToList()
        };

        var @event = eventType.Events.FirstOrDefault(item => item.Identifier == eventId);
        if (@event == null)
        {
            return;
        }

        ViewModel = ViewModel with { EventName = @event.Name };
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId))
            ? FormState.PreviouslyGraded
            : FormState.NotGraded;
    }

    public async Task OnPostAsync(int eventTypeId, int eventId)
    {
        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(eventTypeId))
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
            _databaseContext.ParticipantGrades.Add(new ParticipantGrade
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                GradeTypeId = item.GradeTypeId,
                GradeValue = item.GradeValue
            });
        }

        if (!string.IsNullOrWhiteSpace(FormModel.Note))
        {
            _databaseContext.ParticipantNotes.Add(new ParticipantNote
            {
                EventTypeId = eventTypeId,
                EventId = eventId,
                Note = FormModel.Note
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
