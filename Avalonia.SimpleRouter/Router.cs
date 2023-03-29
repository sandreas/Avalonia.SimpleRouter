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

    public virtual T GoTo<T>() where T : TViewModelBase
    {
        var viewModel = InstantiateViewModel<T>();
        CurrentViewModel = viewModel;
        return viewModel;
    }

    protected T InstantiateViewModel<T>() where T:TViewModelBase
    {
        return (T)Convert.ChangeType(CreateViewModel(typeof(T)), typeof(T));
    }

}