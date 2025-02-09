
namespace Avalonia.SimpleRouter;

public class HistoryRouter<TViewModelBase>: Router<TViewModelBase> where TViewModelBase: class
{
    
    private int _historyIndex = -1;
    private List<TViewModelBase> _history = new();
    private readonly uint _historyMaxSize = 100;

    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;
    
    public IReadOnlyCollection<TViewModelBase> History => _history.AsReadOnly();
    
    public HistoryRouter(Func<Type, TViewModelBase> createViewModel) : base(createViewModel)
    {
    }

    public TViewModelBase? GetHistoryItem(int offset)
    {
        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return default;
        }
        return _history.ElementAt(newIndex);
    }
    
    // pushState
    // popState
    // replaceState

    public void Push(TViewModelBase item)
    {
        // _historyIndex does not point on the last item on push
        // so remove everything after current item to prevent conflicts
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
        }
        
        // add the item and recalculate the index
        _history.Add(item);
        _historyIndex = _history.Count - 1;
        
        // _history exceeded _historyMaxSize, so remove first element and correct index
        if (_history.Count > _historyMaxSize)
        {
            _history.RemoveAt(0);
            _historyIndex--;
        }
    }
    
    public TViewModelBase? Go(int offset = 0)
    {
        // don't navigate if offset is 0 (same viewModel)
        if (offset == 0)
        {
            return default;
        }

        // viewModel == null means offset is invalid
        // _historyIndex can be updated after this without further checks
        var viewModel = GetHistoryItem(offset);
        if (viewModel == null)
        {
            return default;
        }

        _historyIndex += offset;
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
