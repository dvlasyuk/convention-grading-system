using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Database.Entities;

[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public class ExpertGrade
{
    public int Identifier { get; set; }
    public required int EventTypeId { get; set; }
    public required int EventId { get; set; }
    public required int GradeTypeId { get; set; }
    public required int GradeValue { get; set; }
}
