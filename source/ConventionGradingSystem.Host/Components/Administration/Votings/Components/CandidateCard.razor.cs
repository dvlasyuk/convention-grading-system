using ConventionGradingSystem.Host.Components.Administration.Votings.Models;

using Microsoft.AspNetCore.Components;

using MudBlazor;

namespace ConventionGradingSystem.Host.Components.Administration.Votings.Components;

public partial class CandidateCard
{
    [CascadingParameter]
    private IMudDialogInstance DialogInstance { get; set; } = null!;

    [Parameter]
    public CandidateCardParameters Parameters { get; set; } = null!;

    protected override void OnInitialized()
    {
        if (Parameters is not null)
        {
            return;
        }

        DialogInstance.Cancel();
    }
}
