using ConventionGradingSystem.Host.Components.Administration.Votings.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Components;

public partial class CandidateCard
{
    [CascadingParameter]
    private IMudDialogInstance DialogInstance { get; set; } = null!;

    [Parameter]
    public CandidateCardModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (ViewModel is not null)
        {
            return;
        }

        DialogInstance.Cancel();
    }
}
