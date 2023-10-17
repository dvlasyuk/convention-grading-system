using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration;
using ConventionGradingSystem.Models.ParticipantVoteForm;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Pages;

/// <summary>
/// Модель формы приложения для сбора голосов за участников зрительского голосования.
/// </summary>
public class ParticipantVoteFormModel : PageModel
{
    private readonly ApplicationConfiguration _configuration;

    /// <summary>
    /// Создаёт новый экземпляр <see cref="ParticipantVoteFormModel"/>.
    /// </summary>
    /// <param name="configuration">Конфигурационные данные приложения.</param>
    public ParticipantVoteFormModel([NotNull] IOptionsSnapshot<ApplicationConfiguration> configuration) =>
        _configuration = configuration.Value;

    /// <summary>
    /// Состояние формы.
    /// </summary>
    public FormState FormState { get; private set; } = FormState.NotExisted;

    /// <summary>
    /// Модель представления страницы.
    /// </summary>
    public ViewModel ViewModel { get; private set; } = new ViewModel(
        VotingName: "Неизвестное голосование",
        FriendlyVoting: true,
        VotesQuantity: int.MinValue,
        Participants: new List<VotingParticipant>());

    /// <summary>
    /// Обрабатывает GET-запрос к странице.
    /// </summary>
    /// <param name="votingId">Идентификатор голосования.</param>
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
            FriendlyVoting = voting.FriendlyVoting,
            VotesQuantity = voting.VotesQuantity,
            Participants = voting.Participants
                .OrderBy(item => item.Identifier)
                .Select(item => new VotingParticipant(
                    Identifier: item.Identifier,
                    Name: item.Name))
                .ToList()
        };

        FormState = Request.Cookies.Any(item => item.Key == GetCookieName(votingId))
            ? FormState.PreviouslyVoted
            : FormState.NotVoted;
    }

    private static string GetCookieName(string votingId) =>
        $"ParticipantVote-{votingId}";
}
