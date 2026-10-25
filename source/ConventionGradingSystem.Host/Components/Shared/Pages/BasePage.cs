using ConventionGradingSystem.Host.Components.Shared.Models;

using Microsoft.AspNetCore.Components;

namespace ConventionGradingSystem.Host.Components.Shared.Pages;

public abstract class BasePage<TViewModel> : ComponentBase
    where TViewModel : class
{
    protected LoadingStatus LoadingStatus { get; private set; } = LoadingStatus.Loading;
    protected LoadingError? LoadingError { get; private set; }
    protected TViewModel ViewModel
    {
        get => field ?? throw new InvalidOperationException("Content is not loaded");
        private set;
    }

    protected abstract Task<ContentState<TViewModel>> LoadContent();

    protected override async Task OnInitializedAsync()
    {
        var contentState = await LoadContent();
        if (contentState.IsFailed)
        {
            LoadingStatus = LoadingStatus.Error;
            LoadingError = contentState.LoadingError;
            return;
        }

        LoadingStatus = LoadingStatus.Loaded;
        ViewModel = contentState.ViewModel;
    }
}
