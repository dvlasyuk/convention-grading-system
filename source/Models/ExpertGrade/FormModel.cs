namespace ConventionGradingSystem.Models.ExpertGrade;

public record FormModel(
    IReadOnlyCollection<Grade> Grades,
    string Note);

public record Grade(string CriterionId, int GradeValue);
