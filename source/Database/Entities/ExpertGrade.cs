using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Database.Entities;

[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public class ExpertGrade
{
    public int Identifier { get; set; }
    public int EventTypeId { get; set; }
    public int EventId { get; set; }
    public int GradeTypeId { get; set; }
    public int GradeValue { get; set; }
}
