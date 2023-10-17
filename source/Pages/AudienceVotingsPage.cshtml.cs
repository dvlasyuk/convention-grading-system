using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Models.AudienceVotingsPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель страницы приложения со списком участников зрительского голосования.
/// </summary>
[Authorize(Roles = "Adminstrator")]
public class AudienceVotingsPageModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="AudienceVotingsPageModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    public AudienceVotingsPageModel([NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration) =>
        _configuration = configuration.Value;

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
    public void OnGet(string votingId)
    {
        var voting = _configuration.Votings.FirstOrDefault(item => item.Identifier == votingId);
        if (voting == null)
        {
            return;
        }

        ViewModel = ViewModel with
        {
            VotingName = voting.Name,
            Participants = voting.Participants
                .OrderBy(item => item.Identifier)
                .Select(item => new VotingParticipant(
                    Name: item.Name,
                    Brigades: item.Brigades.ToList()))
                .ToList()
        };
    }
}
