namespace ConventionGradingSystem.Models.ParticipantGrade;

public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);

public record Grade(int GradeTypeId, int GradeValue);
