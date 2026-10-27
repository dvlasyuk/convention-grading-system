using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Services;

[SuppressMessage("Design", "CA1024:Use properties where appropriate")]
[SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public static class NavigationHelper
{
    public static string MainPage() => "/";
    public static string SignInPage(string? returnUri = null) => !string.IsNullOrEmpty(returnUri)
        ? $"/sign-in?return_uri={returnUri}"
        : "/sign-in";

    public static class Administration
    {
        public static string ContestsList() => "/administration/contests";
        public static string EventsList(string contestIdentifier) =>
            $"/administration/contests/{contestIdentifier}/events";
        public static string ExpertFeedbacksList(string eventIdentifier) =>
            $"/administration/events/{eventIdentifier}/expert-feedbacks";
        public static string ParticipantFeedbacksList(string eventIdentifier) =>
            $"/administration/events/{eventIdentifier}/participant-feedbacks";

        public static string VotingsList() => "/administration/votings";
        public static string CandidatesList(string votingIdentifier) =>
            $"/administration/votings/{votingIdentifier}/candidates";
        public static string VotesList(string candidateIdentifier) =>
            $"/administration/candidates/{candidateIdentifier}/votes";

        public static string TeamsList() => "/administration/teams";
    }

    public static class Grading
    {
        public static string ExpertFeedbackForm(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/expert-feedback";
        public static string ParticipantFeedbackForm(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/participant-feedback";
        public static string FacilitatorAttendanceForm(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/facilitator-attendance";

        public static string ParticipantVotingForm(string votingIdentifier) =>
            $"/grading/votings/{votingIdentifier}/participant-voting";
    }
}
