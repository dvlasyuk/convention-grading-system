using System.Diagnostics.CodeAnalysis;

namespace ConventionGradingSystem.Host.Components.Shared.Models;

[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types")]
public readonly struct ContentState<TViewModel>
    where TViewModel : class
{
    private readonly TViewModel? _viewModel;
    private readonly LoadingError? _loadingError;

    public bool IsSuccessful => _viewModel is not null;
    public bool IsFailed => _loadingError is not null;

    public TViewModel ViewModel => _viewModel ?? throw new InvalidOperationException("Content state is failed");
    public LoadingError LoadingError => _loadingError ?? throw new InvalidOperationException("Content state is successful");

    private ContentState(TViewModel viewModel)
    {
        _viewModel = viewModel;
        _loadingError = default;
    }

    private ContentState(LoadingError loadingError)
    {
        _viewModel = default;
        _loadingError = loadingError;
    }

    public static implicit operator ContentState<TViewModel>(TViewModel viewModel)
    {
        return new(viewModel);
    }

    public static implicit operator ContentState<TViewModel>(LoadingError loadingError)
    {
        return new(loadingError);
    }
}
