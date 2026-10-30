using ConventionGradingSystem.Host.Components.Administration.Contests.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Contests.Components;

public partial class ContestCard
{
    [CascadingParameter]
    private IMudDialogInstance DialogInstance { get; set; } = null!;

    [Parameter]
    public ContestCardModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (ViewModel is not null)
        {
            return;
        }

        DialogInstance.Cancel();
    }
}
