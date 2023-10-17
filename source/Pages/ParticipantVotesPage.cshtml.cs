using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.ParticipantVotesPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель страницы приложения со списком голосов участников за участника зрительского голосования.
/// </summary>
[Authorize(Roles = "Administrator")]
public class ParticipantVotesPageModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="ParticipantVotesPageModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    /// <param name="databaseContext">Контекст для доступа к базе данных.</param>
    public ParticipantVotesPageModel(
        [NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration,
        [NotNull] DatabaseContext databaseContext)
    {
        _configuration = configuration.Value;
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        VotingName: "Неизвестное голосование",
        ParticipantName: "Неизвестный участник",
        Votes: new List<Vote>());

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="participantId">Идентификатор участника голосования.</param>
    public async Task OnGetAsync(string participantId)
    {
        var votingParticipant = _configuration.Votings
            .SelectMany(item => item.Participants)
            .FirstOrDefault(item => item.Identifier == participantId);

        if (votingParticipant == null)
        {
            return;
        }

        var voting = _configuration.Votings.First(item => item.Participants.Contains(votingParticipant));
        ViewModel = ViewModel with
        {
            VotingName = voting.Name,
            ParticipantName = votingParticipant.Name
        };

        var votes = await _databaseContext.ParticipantVotes
            .Where(item => item.VoitingParticipantId == participantId)
            .ToListAsync();

        var participants = _configuration.Teams
            .SelectMany(item => item.Members)
            .ToDictionary(item => item.Identifier);

        ViewModel = ViewModel with
        {
            Votes = votes
                .OrderBy(item => item.Identifier)
                .Select(item => new Vote(
                    ParticipantName: participants[item.ParticipantId].Name,
                    ParticipantBrigade: participants[item.ParticipantId].Brigade,
                    Note: item.Note))
                .ToList()
        };
    }
}
