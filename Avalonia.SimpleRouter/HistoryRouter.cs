
namespace Avalonia.SimpleRouter;

public class HistoryRouter<TViewModelBase>: Router<TViewModelBase> where TViewModelBase: class
{
    
    private int _historyIndex = -1;
    private List<TViewModelBase> _history = new();
    private readonly uint _historyMaxSize = 100;

    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;
    
    public HistoryRouter(Func<Type, TViewModelBase> createViewModel) : base(createViewModel)
    {
    }
    
    // pushState
    // popState
    // replaceState

    public void Push(TViewModelBase item)
    {
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
        }
        _history.Add(item);
        _historyIndex = _history.Count - 1;
        if (_history.Count > _historyMaxSize)
        {
            _history.RemoveAt(0);
        }
    }
    
    public TViewModelBase? Go(int offset = 0)
    {
        if (offset == 0)
        {
            return default;
        }

        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return default;
        }
        _historyIndex = newIndex;
        var viewModel = _history.ElementAt(_historyIndex);
        CurrentViewModel = viewModel;
        return viewModel;
    }
    
    public TViewModelBase? Back() => HasPrev ? Go(-1) : default;
    
    public TViewModelBase? Forward() => HasNext ? Go(1) : default;
    
    
    public override T GoTo<T>()
    {
        var destination = InstantiateViewModel<T>();
        CurrentViewModel = destination;
        Push(destination);
        return destination;
    }
}
