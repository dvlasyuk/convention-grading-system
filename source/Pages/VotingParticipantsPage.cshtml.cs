using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Database;
using ConventionGradingSystem.Models.VotingParticipantsPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель страницы приложения со списком участников зрительского голосования.
/// </summary>
[Authorize(Roles = "Administrator")]
public class VotingParticipantsPageModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;
    private readonly DatabaseContext _databaseContext;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="VotingParticipantsPageModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    /// <param name="databaseContext">Контекст для доступа к базе данных.</param>
    public VotingParticipantsPageModel(
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
        Participants: new List<VotingParticipant>());

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="votingId">Идентификатор зрительского голосования.</param>
    public async Task OnGetAsync(string votingId)
    {
        var voting = _configuration.Votings.FirstOrDefault(item => item.Identifier == votingId);
        if (voting == null)
        {
            return;
        }

        var participantIds = voting.Participants
            .Select(item => item.Identifier)
            .ToList();

        var participantVotes = await _databaseContext.ParticipantVotes
            .Where(item => participantIds.Contains(item.VoitingParticipantId))
            .ToListAsync();

        var votesQuantities = participantVotes
            .GroupBy(item => item.VoitingParticipantId)
            .ToDictionary(
                groupByEvent => groupByEvent.Key,
                groupByEvent => groupByEvent.Count());

        ViewModel = ViewModel with
        {
            VotingName = voting.Name,
            Participants = voting.Participants
                .OrderBy(item => item.Identifier)
                .Select(item => new VotingParticipant(
                    Identifier: item.Identifier,
                    Name: item.Name,
                    VotesQuantity: votesQuantities.TryGetValue(item.Identifier, out var quantity)
                        ? quantity
                        : 0,
                    Brigades: item.Brigades.ToList()))
                .ToList()
        };
    }
}
