using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Configuration.Validators;

/// <summary>
/// Валидатор конфигурационных данных приложения.
/// </summary>
public class ApplicationConfigurationValidator : IValidateOptions<ApplicationConfiguration>
{
    /// <summary>
    /// Валидирует заданный экземпляр конфигурационных данных.
    /// </summary>
    /// <param name="name">Название экземпляра данных для валидации.</param>
    /// <param name="options">Экземпляр данных для валидации.</param>
    /// <returns>Результат валидации.</returns>
    public ValidateOptionsResult Validate(string? name, [NotNull] ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();

        var participantIds = options.Teams
            .SelectMany(team => team.Members)
            .Select(participant => participant.Identifier)
            .ToList();

        failureMessages.AddRange(ValidateIdentifiersUniqueness(options));
        failureMessages.AddRange(options.Contests.SelectMany(item => ValidateContest(item, participantIds)));
        failureMessages.AddRange(options.Votings.SelectMany(ValidateAudienceVoting));
        failureMessages.AddRange(options.Teams.SelectMany(ValidateTeam));

        return failureMessages.Any()
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }

    private static IEnumerable<string> ValidateIdentifiersUniqueness(ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор конкурса",
            values: options.Contests.Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор мероприятия",
            values: options.Contests
                .SelectMany(item => item.Events)
                .Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор критерия оценивания экспертами",
            values: options.Contests
                .SelectMany(item => item.ExpertCriterions)
                .Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор критерия оценивания участниками",
            values: options.Contests
                .SelectMany(item => item.ParticipantCriterions)
                .Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор зрительского голосования",
            values: options.Votings.Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор участника зрительского голосования",
            values: options.Votings
                .SelectMany(item => item.Participants)
                .Select(item => item.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор команды",
            values: options.Teams.Select(team => team.Identifier),
            validator: IsValidIdentifier));

        failureMessages.AddRange(ValidateUniqueness(
            name: "Идентификатор участника",
            values: options.Teams
                .SelectMany(team => team.Members)
                .Select(participant => participant.Identifier),
            validator: IsValidIdentifier));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateContest(Contest contest, ICollection<string> participantIds)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(contest.Identifier))
        {
            failureMessages.Add("Для одного из конкурсов задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(contest.Name))
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано пустое название");
        }
        else if (contest.Name.Length > 100)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано название, превышающее 100 символов");
        }

        if (contest.ExpertCriterions.Count == 0 && contest.ParticipantCriterions.Count == 0)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} не задано ни одного критерия оценивания");
        }
        if (contest.ExpertCriterions.Count > 10)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано более 10 критериев оценивания экспертами");
        }
        if (contest.ParticipantCriterions.Count > 10)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано более 10 критериев оценивания участниками");
        }

        failureMessages.AddRange(contest.ExpertCriterions.SelectMany(ValidateGradeCriterion));
        failureMessages.AddRange(contest.ParticipantCriterions.SelectMany(ValidateGradeCriterion));

        if (contest.Events.Count == 0)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} не задано ни одного мероприятия");
        }
        if (contest.Events.Count > 100)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано более 100 мероприятий");
        }

        failureMessages.AddRange(contest.Events.SelectMany(item => ValidateContestEvent(item, participantIds)));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateGradeCriterion(GradeCriterion criterion)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(criterion.Identifier))
        {
            failureMessages.Add("Для одного из критериев оценивания задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(criterion.Name))
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задано пустое название");
        }
        else if (criterion.Name.Length > 100)
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задано название, превышающее 100 символов");
        }

        if (string.IsNullOrWhiteSpace(criterion.Description))
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задано пустое описание");
        }
        else if (criterion.Name.Length > 1000)
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задано описание, превышающее 1000 символов");
        }

        if (criterion.MinimalGrade < 0)
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задана отрицательная минимальная оценка");
        }
        if (criterion.MaximalGrade < 0)
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задана отрицательная максимальная оценка");
        }
        if (criterion.MinimalGrade >= criterion.MaximalGrade)
        {
            failureMessages.Add($"Для критерия оценивания {criterion.Identifier} задана минимальная оценка, большая или равная максимальной");
        }

        return failureMessages;
    }

    private static IEnumerable<string> ValidateContestEvent(ContestEvent contestEvent, ICollection<string> participantIds)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(contestEvent.Identifier))
        {
            failureMessages.Add("Для одного из мероприятий задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(contestEvent.Name))
        {
            failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задано пустое название");
        }
        else if (contestEvent.Name.Length > 100)
        {
            failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задано название, превышающее 100 символов");
        }

        if (contestEvent.Participants.Count == 0)
        {
            failureMessages.Add($"Для мероприятия {contestEvent.Identifier} не задано ни одного участника");
        }
        if (contestEvent.Participants.Count > 100)
        {
            failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задано более 100 участников");
        }

        foreach (var participant in contestEvent.Participants)
        {
            if (!IsValidIdentifier(participant))
            {
                failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задан пустой или слишком длинный идентификатор участника");
                continue;
            }
            if (!participantIds.Contains(participant))
            {
                failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задан несуществующий идентификатор участника");
            }
        }

        failureMessages.AddRange(ValidateUniqueness(
            name: $"Идентификатор участника для мероприятия {contestEvent.Identifier}",
            values: contestEvent.Participants,
            validator: IsValidIdentifier));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateAudienceVoting(AudienceVoting voting)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(voting.Identifier))
        {
            failureMessages.Add("Для одного из зрительских голосований задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(voting.Name))
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано пустое название");
        }
        else if (voting.Name.Length > 100)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано название, превышающее 100 символов");
        }

        if (voting.VotesQuantity < 0)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано отрицательное количество голосов");
        }
        if (voting.VotesQuantity >= voting.Participants.Count)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано количество голосов, большее или равное количеству участников");
        }

        if (voting.Participants.Count == 0)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} не задано ни одного участника");
        }
        if (voting.Participants.Count > 100)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано более 100 участников");
        }

        failureMessages.AddRange(voting.Participants.SelectMany(ValidateVotingParticipant));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateVotingParticipant(VotingParticipant participant)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(participant.Identifier))
        {
            failureMessages.Add("Для одного из участников голосований задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(participant.Name))
        {
            failureMessages.Add($"Для участника голосования {participant.Identifier} задано пустое название/имя");
        }
        else if (participant.Name.Length > 100)
        {
            failureMessages.Add($"Для участника голосования {participant.Identifier} задано название/имя, превышающее 100 символов");
        }

        if (participant.Brigades.Count == 0)
        {
            failureMessages.Add($"Для участника голосования {participant.Identifier} не задано ни одного связанного отряда");
        }
        if (participant.Brigades.Count > 10)
        {
            failureMessages.Add($"Для участника голосования {participant.Identifier} задано более 10 связанных отрядов");
        }

        foreach (var brigade in participant.Brigades)
        {
            if (string.IsNullOrWhiteSpace(brigade))
            {
                failureMessages.Add($"Для участника голосования {participant.Identifier} задано пустое название связанного отряда");
            }
            else if (brigade.Length > 100)
            {
                failureMessages.Add($"Для участника голосования {participant.Identifier} задано название связанного отряда, превышающее 100 символов");
            }
        }

        failureMessages.AddRange(ValidateUniqueness(
            name: $"Название связанного отряда для участника голосования {participant.Identifier}",
            values: participant.Brigades,
            validator: value => !string.IsNullOrWhiteSpace(value) && value.Length <= 100));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateTeam(Team team)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(team.Identifier))
        {
            failureMessages.Add("Для одной из команд задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(team.Name))
        {
            failureMessages.Add($"Для команды {team.Identifier} задано пустое название");
        }
        else if (team.Name.Length > 100)
        {
            failureMessages.Add($"Для команды {team.Identifier} задано название, превышающее 100 символов");
        }

        if (team.Members.Count == 0)
        {
            failureMessages.Add($"Для команды {team.Identifier} не задано ни одного участника");
        }
        if (team.Members.Count > 100)
        {
            failureMessages.Add($"Для команды {team.Identifier} задано более 100 участников");
        }

        failureMessages.AddRange(team.Members.SelectMany(ValidateParticipant));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateParticipant(Participant participant)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(participant.Identifier))
        {
            failureMessages.Add("Для одного из участников задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(participant.Name))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано пустое имя");
        }
        else if (participant.Name.Length > 100)
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано имя, превышающее 100 символов");
        }

        if (string.IsNullOrWhiteSpace(participant.Brigade))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано пустое название отряда");
        }
        else if (participant.Brigade.Length > 100)
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано название отряда, превышающее 100 символов");
        }

        return failureMessages;
    }

    private static bool IsValidIdentifier(string identifier) =>
        !string.IsNullOrWhiteSpace(identifier) && identifier.Length <= 50;

    private static IEnumerable<string> ValidateUniqueness(string name, IEnumerable<string> values, Func<string, bool> validator)
    {
        return values
            .Where(validator)
            .GroupBy(identifier => identifier)
            .Where(group => group.Count() > 1)
            .Select(group => $"{name} {group.Key} повторяется {group.Count()} раз")
            .ToList();
    }
}
