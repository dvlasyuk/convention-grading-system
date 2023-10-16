using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.Configuration.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.Configuration.Validators;

public class ApplicationConfigurationValidator : IValidateOptions<ApplicationConfiguration>
{
    public ValidateOptionsResult Validate(string? name, [NotNull] ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();

        failureMessages.AddRange(ValidateIdentifiersUniqueness(options));
        failureMessages.AddRange(options.Contests.SelectMany(ValidateContest));
        failureMessages.AddRange(options.Teams.SelectMany(ValidateTeam));

        return failureMessages.Any()
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }

    private static IEnumerable<string> ValidateIdentifiersUniqueness(ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор конкурса",
            identifiers: options.Contests.Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор мероприятия",
            identifiers: options.Contests
                .SelectMany(item => item.Events)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор критерия оценивания экспертами",
            identifiers: options.Contests
                .SelectMany(item => item.ExpertCriterions)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор критерия оценивания участниками",
            identifiers: options.Contests
                .SelectMany(item => item.ParticipantCriterions)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор команды",
            identifiers: options.Teams.Select(team => team.Identifier)));

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: "Идентификатор участника",
            identifiers: options.Teams
                .SelectMany(team => team.Members)
                .Select(participant => participant.Identifier)));

        return failureMessages;
    }

    private static IEnumerable<string> ValidateContest(Contest contest)
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

        failureMessages.AddRange(contest.Events.SelectMany(ValidateContestEvent));

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

    private static IEnumerable<string> ValidateContestEvent(ContestEvent contestEvent)
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

        failureMessages.AddRange(ValidateIdentifiersUniqueness(
            name: $"Идентификатор участника для мероприятия {contestEvent.Identifier}",
            identifiers: contestEvent.Participants));

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

        if (string.IsNullOrWhiteSpace(participant.Name))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано пустое название отряда");
        }
        else if (participant.Name.Length > 100)
        {
            failureMessages.Add($"Для участника {participant.Identifier} задано название отряда, превышающее 100 символов");
        }

        return failureMessages;
    }

    private static bool IsValidIdentifier(string identifier) =>
        !string.IsNullOrWhiteSpace(identifier) || identifier.Length > 50;

    private static IEnumerable<string> ValidateIdentifiersUniqueness(string name, IEnumerable<string> identifiers)
    {
        return identifiers
            .Where(IsValidIdentifier)
            .GroupBy(identifier => identifier)
            .Where(group => group.Count() > 1)
            .Select(group => $"{name} {group.Key} повторяется {group.Count()} раз")
            .ToList();
    }
}
