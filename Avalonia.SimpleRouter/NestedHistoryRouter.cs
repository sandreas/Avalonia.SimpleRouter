using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter;

public class NestedHistoryRouter<TViewModelBase, TMainViewModel> 
    where TViewModelBase : ISimpleRoute<TViewModelBase>
    where TMainViewModel: TViewModelBase
{
    // ReSharper disable once StaticMemberInGenericType (Reason: NestedHistoryRouter is Singleton)
    // In the vast majority of cases, having a static field in a generic type is a sign of an error. The reason for this is that a static field in a generic type will not be shared among instances of different close constructed types. This means that for a generic class C<T> which has a static field X, the values of C<int>.X and C<string>.X have completely different, independent values.
    private static bool _viewModelTreeUpdateInProgress;

    private List<TViewModelBase> _currentViewModelTree = new();
    
    protected readonly Func<Type, TViewModelBase> CreateViewModel;
    public event Action<TViewModelBase[]>? CurrentViewModelsChanged;
    

    private int _historyIndex = -1;
    private List<TViewModelBase[]> _history = new();
    private readonly uint _historyMaxSize = 100;
    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;
    
    public IReadOnlyCollection<TViewModelBase[]> History => _history.AsReadOnly();



    public NestedHistoryRouter( Func<Type, TViewModelBase> createViewModel)
    {
        CreateViewModel = createViewModel;
    }

    
    private static bool AreViewModelsEqual(TViewModelBase? first, TViewModelBase? second) => EqualityComparer<TViewModelBase?>.Default.Equals(first, second);


    private void UpdateViewModelTree(params TViewModelBase[] viewModels)
    {
        if (viewModels.Length == 0)
        {
            return;
        }
        TViewModelBase parent = viewModels.First();
        var updatedTree = new List<TViewModelBase> {parent};

        foreach (var child in viewModels.Skip(1))
        {
            if (!AreViewModelsEqual(parent.Content,child) )
            {
                parent.Content = child;
            }

            parent = child;
            updatedTree.Add(child);
        }

        _currentViewModelTree = updatedTree;
        CurrentViewModelsChanged?.Invoke(updatedTree.ToArray());
    }

    private void UpdateViewModelTree(params Type[] viewModelTypes)
    {
        // prevent recursion due to lazy initialization of view models
        if (_viewModelTreeUpdateInProgress)
        {
            return;
        }

        _viewModelTreeUpdateInProgress = true;
        
        try
        {
            var viewModelTypesList = viewModelTypes.ToList();
            if (viewModelTypes.FirstOrDefault() != typeof(TMainViewModel))
            {
                viewModelTypesList.Insert(0, typeof(TMainViewModel));
            }

            var newViewModelTree = new List<TViewModelBase>();
            var counter = 0;
            foreach (var type in viewModelTypesList)
            {
                // viewModels can be reused on the same level
                // or must be created if they don't exist
                var newViewModel = _currentViewModelTree.Count >= viewModelTypesList.Count && _currentViewModelTree.ElementAt(counter).GetType() == type
                    ? _currentViewModelTree.ElementAt(counter)
                    : CreateViewModel(type);
                
                newViewModelTree.Add(newViewModel);
                counter++;
            }
            UpdateViewModelTree(newViewModelTree.ToArray());

        }
        finally
        {
            _viewModelTreeUpdateInProgress = false;
        }
    }

    public TViewModelBase[] GetHistoryItem(int offset)
    {
        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return [];
        }
        return _history.ElementAt(newIndex);
    }
    
    private void UpdateHistory(TViewModelBase[] stack)
    {
        // _historyIndex does not point on the last item on push
        // so remove everything after current item to prevent conflicts
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
        }
        
        // add the item and recalculate the index
        _history.Add(stack);
        _historyIndex = _history.Count - 1;
        
        // _history exceeded _historyMaxSize, so remove first element and correct index
        if (_history.Count > _historyMaxSize)
        {
            _history.RemoveAt(0);
            _historyIndex--;
        }
    }
    
    public TViewModelBase[] Go(int offset = 0)
    {
        // don't navigate if offset is 0 (same viewModel)
        if (offset == 0)
        {
            return _currentViewModelTree.ToArray();
        }

        // viewModel == null means offset is invalid
        // _historyIndex can be updated after this without further checks
        var historyItem = GetHistoryItem(offset);
        if (!historyItem.Any())
        {
            return historyItem;
        }
        
        _historyIndex += offset;
        if (historyItem.Length > 1)
        {
            UpdateViewModelTree(historyItem);
        }

        return historyItem;

    }
    
    public TViewModelBase[] Back() => HasPrev ? Go(-1) : _currentViewModelTree.ToArray();
    
    public TViewModelBase[] Forward() => HasNext ? Go(1) : _currentViewModelTree.ToArray();
    
    public TViewModelBase[] GoTo<T>() 
        where T : TViewModelBase => Goto(typeof(T));
    
    public TViewModelBase[] GoTo<T1, T2>() 
        where T1 : TViewModelBase 
        where T2: TViewModelBase => Goto(typeof(T1), typeof(T2));

    public TViewModelBase[] GoTo<T1, T2, T3>() 
        where T1 : TViewModelBase 
        where T2 : TViewModelBase 
        where T3: TViewModelBase => Goto(typeof(T1), typeof(T2), typeof(T3));
    
    public TViewModelBase[] Goto(params Type[] viewModelTypes)
    {
        UpdateViewModelTree(viewModelTypes);
        UpdateHistory(_currentViewModelTree.ToArray());
        return _currentViewModelTree.ToArray();
    }
    
     
    
}