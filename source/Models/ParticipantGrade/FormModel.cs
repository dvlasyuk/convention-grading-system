namespace ConventionGradingSystem.Models.ParticipantGrade;

public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);

public record Grade(int CriterionId, int GradeValue);
