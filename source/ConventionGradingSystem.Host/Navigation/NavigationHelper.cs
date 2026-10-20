using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Navigation;

[SuppressMessage("Design", "CA1024:Use properties where appropriate")]
[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public static class NavigationHelper
{
    public static string GetMainPageLink() => "/";

    public static class Administration
    {
        public static string GetContestsListLink() => "/administration/contests";
        public static string GetEventsListLink(string contestIdentifier) =>
            $"/administration/contests/{contestIdentifier}/events";
        public static string GetExpertFeedbacksListLink(string eventIdentifier) =>
            $"/administration/events/{eventIdentifier}/expert-feedbacks";
        public static string GetParticipantFeedbacksListLink(string eventIdentifier) =>
            $"/administration/events/{eventIdentifier}/participant-feedbacks";

        public static string GetVotingsListLink() => "/administration/votings";
        public static string GetCandidatesListLink(string votingIdentifier) =>
            $"/administration/votings/{votingIdentifier}/candidates";
        public static string GetVotesListLink(string candidateIdentifier) =>
            $"/administration/candidates/{candidateIdentifier}/votes";

        public static string GetTeamsListLink() => "/administration/teams";
    }

    public static class Grading
    {
        public static string GetExpertFeedbackFormLink(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/expert-feedback";
        public static string GetParticipantFeedbackFormLink(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/participant-feedback";
        public static string GetFacilitatorAttendanceFormLink(string eventIdentifier) =>
            $"/grading/events/{eventIdentifier}/facilitator-attendance";

        public static string GetParticipantVotingFormLink(string votingIdentifier) =>
            $"/grading/votings/{votingIdentifier}/participant-voting";
    }
}
