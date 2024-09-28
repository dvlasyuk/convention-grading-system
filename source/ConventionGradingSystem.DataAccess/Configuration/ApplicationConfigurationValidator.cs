using System.Diagnostics.CodeAnalysis;

using ConventionGradingSystem.DataAccess.Configuration.Models;

using Microsoft.Extensions.Options;

namespace ConventionGradingSystem.DataAccess.Configuration;

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

        failureMessages.AddRange(ValidateIdentifiersUniqueness(options));
        failureMessages.AddRange(options.Contests.SelectMany(item => ValidateContest(item, options.Teams, options.Brigades, options.Participants)));
        failureMessages.AddRange(options.Votings.SelectMany(item => ValidateVoting(item, options.Brigades, options.Teams)));
        failureMessages.AddRange(options.Teams.SelectMany(item => ValidateTeam(item, options.Participants)));
        failureMessages.AddRange(options.Brigades.SelectMany(item => ValidateBrigade(item, options.Participants)));
        failureMessages.AddRange(options.Participants.SelectMany(item => ValidateParticipant(item, options.Brigades, options.Teams)));
        failureMessages.AddRange(options.Experts.SelectMany(ValidateExpert));

        return failureMessages.Count > 0
            ? ValidateOptionsResult.Fail(failureMessages)
            : ValidateOptionsResult.Success;
    }

    private static List<string> ValidateIdentifiersUniqueness(ApplicationConfiguration options)
    {
        var failureMessages = new List<string>();

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор конкурса",
            values: options.Contests
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор мероприятия",
            values: options.Contests
                .SelectMany(item => item.Events)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор критерия оценивания экспертами",
            values: options.Contests
                .SelectMany(item => item.ExpertCriterions)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор критерия оценивания участниками",
            values: options.Contests
                .SelectMany(item => item.ParticipantCriterions)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор зрительского голосования",
            values: options.Votings
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор кандидата зрительского голосования",
            values: options.Votings
                .SelectMany(item => item.Candidates)
                .Select(item => item.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор команды",
            values: options.Teams
                .Select(team => team.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор отряда",
            values: options.Brigades
                .Select(team => team.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор участника",
            values: options.Participants
                .Select(participant => participant.Identifier)));

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: "Идентификатор эксперта",
            values: options.Experts
                .Select(expert => expert.Identifier)));

        return failureMessages;
    }

    private static List<string> ValidateContest(
        Contest contest,
        ICollection<Team> teams,
        ICollection<Brigade> brigades,
        ICollection<Participant> participants)
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

        if (contest.BannedTeams.Count > 100)
        {
            failureMessages.Add($"Для конкурса {contest.Identifier} задано более 100 исключённых команд");
        }

        foreach (var team in contest.BannedTeams)
        {
            if (!IsValidIdentifier(team))
            {
                failureMessages.Add($"Для конкурса {contest.Identifier} задан пустой или слишком длинный идентификатор исключённой команды");
                continue;
            }
            if (!teams.Any(item => item.Identifier == team))
            {
                failureMessages.Add($"Для конкурса {contest.Identifier} задан несуществующий идентификатор исключённой команды");
            }
        }

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: $"Идентификатор исключённой команды для конкурса {contest.Identifier}",
            values: contest.BannedTeams));

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

        failureMessages.AddRange(contest.Events.SelectMany(item => ValidateContestEvent(
            contestEvent: item,
            brigades: brigades,
            participants: participants,
            registeredGrading: contest.RegisteredGrading,
            friendlyGrading: contest.FriendlyGrading,
            attendanceControl: contest.AttendanceControl)));

        return failureMessages;
    }

    private static List<string> ValidateGradeCriterion(GradeCriterion criterion)
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

    private static List<string> ValidateContestEvent(
        ContestEvent contestEvent,
        ICollection<Brigade> brigades,
        ICollection<Participant> participants,
        bool registeredGrading,
        bool friendlyGrading,
        bool attendanceControl)
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

        if (!friendlyGrading)
        {
            if (contestEvent.Brigades.Count == 0)
            {
                failureMessages.Add($"Для мероприятия {contestEvent.Identifier} не задано ни одного связанного отряда");
            }
            if (contestEvent.Brigades.Count > 10)
            {
                failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задано более 10 связанных отрядов");
            }

            foreach (var brigade in contestEvent.Brigades)
            {
                if (!IsValidIdentifier(brigade))
                {
                    failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задан пустой или слишком длинный идентификатор связанного отряда");
                }
                if (!brigades.Any(item => item.Identifier == brigade))
                {
                    failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задан несуществующий идентификатор связанного отряда");
                }
            }

            failureMessages.AddRange(ValidateIdentifierUniqueness(
                name: $"Идентификатор связанного отряда для мероприятия {contestEvent.Identifier}",
                values: contestEvent.Brigades));
        }

        if (registeredGrading || attendanceControl)
        {
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
                if (!participants.Any(item => item.Identifier == participant))
                {
                    failureMessages.Add($"Для мероприятия {contestEvent.Identifier} задан несуществующий идентификатор участника");
                }
            }

            failureMessages.AddRange(ValidateIdentifierUniqueness(
                name: $"Идентификатор участника для мероприятия {contestEvent.Identifier}",
                values: contestEvent.Participants));
        }

        return failureMessages;
    }

    private static List<string> ValidateVoting(
        Voting voting,
        ICollection<Brigade> brigades,
        ICollection<Team> teams)
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
        if (voting.VotesQuantity >= voting.Candidates.Count)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано количество голосов, большее или равное количеству кандидатов");
        }

        if (voting.BannedTeams.Count > 100)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано более 100 исключённых команд");
        }

        foreach (var team in voting.BannedTeams)
        {
            if (!IsValidIdentifier(team))
            {
                failureMessages.Add($"Для зрительского голосования {voting.Identifier} задан пустой или слишком длинный идентификатор исключённой команды");
                continue;
            }
            if (!teams.Any(item => item.Identifier == team))
            {
                failureMessages.Add($"Для зрительского голосования {voting.Identifier} задан несуществующий идентификатор исключённой команды");
            }
        }

        failureMessages.AddRange(ValidateIdentifierUniqueness(
            name: $"Идентификатор исключённой команды для зрительского голосования {voting.Identifier}",
            values: voting.BannedTeams));

        if (voting.Candidates.Count == 0)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} не задано ни одного кандидата");
        }
        if (voting.Candidates.Count > 100)
        {
            failureMessages.Add($"Для зрительского голосования {voting.Identifier} задано более 100 кандидатов");
        }

        failureMessages.AddRange(voting.Candidates.SelectMany(item => ValidateCandidate(
            candidate: item,
            brigades: brigades,
            friendlyVoting: voting.FriendlyVoting)));

        return failureMessages;
    }

    private static List<string> ValidateCandidate(
        Candidate candidate,
        ICollection<Brigade> brigades,
        bool friendlyVoting)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(candidate.Identifier))
        {
            failureMessages.Add("Для одного из кандидатов голосований задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(candidate.Name))
        {
            failureMessages.Add($"Для кандидата голосования {candidate.Identifier} задано пустое название/имя");
        }
        else if (candidate.Name.Length > 100)
        {
            failureMessages.Add($"Для кандидата голосования {candidate.Identifier} задано название/имя, превышающее 100 символов");
        }

        if (!friendlyVoting)
        {
            if (candidate.Brigades.Count == 0)
            {
                failureMessages.Add($"Для кандидата голосования {candidate.Identifier} не задано ни одного связанного отряда");
            }
            if (candidate.Brigades.Count > 10)
            {
                failureMessages.Add($"Для кандидата голосования {candidate.Identifier} задано более 10 связанных отрядов");
            }

            foreach (var brigade in candidate.Brigades)
            {
                if (!IsValidIdentifier(brigade))
                {
                    failureMessages.Add($"Для кандидата голосования {candidate.Identifier} задан пустой или слишком длинный идентификатор связанного отряда");
                }
                if (!brigades.Any(item => item.Identifier == brigade))
                {
                    failureMessages.Add($"Для кандидата голосования {candidate.Identifier} задан несуществующий идентификатор связанного отряда");
                }
            }

            failureMessages.AddRange(ValidateIdentifierUniqueness(
                name: $"Идентификатор связанного отряда для кандидата голосования {candidate.Identifier}",
                values: candidate.Brigades));
        }

        return failureMessages;
    }

    private static List<string> ValidateTeam(Team team, ICollection<Participant> participants)
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

        var members = participants
            .Where(participant => participant.Team == team.Identifier)
            .ToList();

        if (members.Count == 0)
        {
            failureMessages.Add($"Для команды {team.Identifier} не задано ни одного участника");
        }
        if (members.Count > 100)
        {
            failureMessages.Add($"Для команды {team.Identifier} задано более 100 участников");
        }

        return failureMessages;
    }

    private static List<string> ValidateBrigade(Brigade brigade, ICollection<Participant> participants)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(brigade.Identifier))
        {
            failureMessages.Add("Для одного из отрядов задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(brigade.Name))
        {
            failureMessages.Add($"Для отряда {brigade.Identifier} задано пустое название");
        }
        else if (brigade.Name.Length > 100)
        {
            failureMessages.Add($"Для отряда {brigade.Identifier} задано название, превышающее 100 символов");
        }

        var members = participants
            .Where(participant => participant.Brigade == brigade.Identifier)
            .ToList();

        if (members.Count > 100)
        {
            failureMessages.Add($"Для отряда {brigade.Identifier} задано более 100 участников");
        }

        return failureMessages;
    }

    private static List<string> ValidateParticipant(
        Participant participant,
        ICollection<Brigade> brigades,
        ICollection<Team> teams)
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

        if (!IsValidIdentifier(participant.Brigade))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задан пустой или слишком длинный идентификатор отряда");
        }
        if (!brigades.Any(item => item.Identifier == participant.Brigade))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задан несуществующий идентификатор отряда");
        }

        if (!IsValidIdentifier(participant.Team))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задан пустой или слишком длинный идентификатор команды");
        }
        if (!teams.Any(item => item.Identifier == participant.Team))
        {
            failureMessages.Add($"Для участника {participant.Identifier} задан несуществующий идентификатор команды");
        }

        return failureMessages;
    }

    private static List<string> ValidateExpert(Expert expert)
    {
        var failureMessages = new List<string>();

        if (!IsValidIdentifier(expert.Identifier))
        {
            failureMessages.Add("Для одного из экспертов задан пустой или слишком длинный идентификатор");
            return failureMessages;
        }

        if (string.IsNullOrWhiteSpace(expert.Name))
        {
            failureMessages.Add($"Для эксперта {expert.Identifier} задано пустое имя");
        }
        else if (expert.Name.Length > 100)
        {
            failureMessages.Add($"Для эксперта {expert.Identifier} задано имя, превышающее 100 символов");
        }

        return failureMessages;
    }

    private static bool IsValidIdentifier(string identifier) =>
        !string.IsNullOrWhiteSpace(identifier) && identifier.Length <= 50;

    private static List<string> ValidateIdentifierUniqueness(string name, IEnumerable<string> values)
    {
        return values
            .Where(IsValidIdentifier)
            .GroupBy(identifier => identifier)
            .Where(group => group.Count() > 1)
            .Select(group => $"{name} {group.Key} повторяется {group.Count()} раз")
            .ToList();
    }
}
