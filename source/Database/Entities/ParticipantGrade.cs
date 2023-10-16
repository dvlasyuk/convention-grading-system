using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Database.Entities;

[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public class ParticipantGrade
{
    public int Identifier { get; set; }
    public required string EventId { get; set; }
    public required string CriterionId { get; set; }
    public required int GradeValue { get; set; }
}
