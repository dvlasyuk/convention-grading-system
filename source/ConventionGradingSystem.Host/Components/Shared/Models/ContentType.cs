using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Components.Shared.Models;

[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public enum ContentType
{
    Default = 0,
    Table = 1,
    Form = 2,
    TableForm = 3
}
