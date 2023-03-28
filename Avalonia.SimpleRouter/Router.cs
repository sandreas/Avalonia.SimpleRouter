namespace Avalonia.SimpleRouter;

public class Router<TViewModelBase> where TViewModelBase:class
{

    private TViewModelBase _currentViewModel = default!;
    protected readonly Func<Type, TViewModelBase> CreateViewModel;
    public event Action<TViewModelBase>? CurrentViewModelChanged;

    public Router(Func<Type, TViewModelBase> createViewModel)
    {
        CreateViewModel = createViewModel;
    }

    protected TViewModelBase CurrentViewModel
    {
        set
        {
            if (value == _currentViewModel) return;
            _currentViewModel = value;
            OnCurrentViewModelChanged(value);
        }
    }

    private void OnCurrentViewModelChanged(TViewModelBase viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }

    public virtual void GoTo<T>() where T : TViewModelBase
    {
        CurrentViewModel = CreateViewModel(typeof(T));
    }

}