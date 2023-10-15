using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Database.Entities;

[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public class ParticipantGrade
{
    public int Identifier { get; set; }
    public required int ContestId { get; set; }
    public required int EventId { get; set; }
    public required int CriterionId { get; set; }
    public required int GradeValue { get; set; }
}
