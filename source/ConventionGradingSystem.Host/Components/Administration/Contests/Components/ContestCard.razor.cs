using ConventionGradingSystem.Host.Components.Administration.Contests.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Components;

public partial class ContestCard
{
    [CascadingParameter]
    private IMudDialogInstance DialogInstance { get; set; } = null!;

    [Parameter]
    public ContestCardParameters Parameters { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (Parameters is not null)
        {
            return;
        }

        DialogInstance.Cancel();
    }
}
