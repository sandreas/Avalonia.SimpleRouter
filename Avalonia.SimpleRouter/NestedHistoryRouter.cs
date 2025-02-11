using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter;

public class NestedHistoryRouter<TViewModelBase, TMainViewModel> 
    where TViewModelBase : ISimpleRoute<TViewModelBase>
    where TMainViewModel: TViewModelBase
{

    
    // ReSharper disable once StaticMemberInGenericType (Reason: NestedHistoryRouter is Singleton)
    // In the vast majority of cases, having a static field in a generic type is a sign of an error. The reason for this is that a static field in a generic type will not be shared among instances of different close constructed types. This means that for a generic class C<T> which has a static field X, the values of C<int>.X and C<string>.X have completely different, independent values.
    private static bool _stackUpdateInProgress;
    
    private int _historyIndex = -1;
    private List<TViewModelBase[]> _history = new();
    private readonly uint _historyMaxSize = 100;
    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;
    
    public IReadOnlyCollection<TViewModelBase[]> History => _history.AsReadOnly();
    private List<TViewModelBase> _currentViewModelPack = new();
    
    protected readonly Func<Type, TViewModelBase> CreateViewModel;

    public NestedHistoryRouter( Func<Type, TViewModelBase> createViewModel)
    {
        CreateViewModel = createViewModel;
    }

    
    private static bool AreViewModelsEqual(TViewModelBase? first, TViewModelBase? second) => EqualityComparer<TViewModelBase?>.Default.Equals(first, second);

    
    private void UpdateCurrentViewModelStack(params Type[] viewModelTypes)
    {
        // prevent recursion when used in constructor
        if (_stackUpdateInProgress)
        {
            return;
        }
        
        try
        {
            _stackUpdateInProgress = true;
            if (_currentViewModelPack.Count == 0)
            {
                _currentViewModelPack.Add(CreateViewModel(typeof(TMainViewModel)));
            }

            // clone the original stack
            var viewModelTree = _currentViewModelPack.ToArray();
            TViewModelBase parentViewModel = viewModelTree.First();
            var newViewModelTree = new List<TViewModelBase>(){parentViewModel};
            var counter = 1; // start on 1 because MainViewModel is excluded in viewModelTypes
            foreach (var type in viewModelTypes)
            {
                // viewModels can be reused on the same level
                var newViewModel = viewModelTree.Length > counter && viewModelTree.ElementAt(counter).GetType() == type
                    ? viewModelTree.ElementAt(counter)
                    : CreateViewModel(type); // this has to be 
                if (!AreViewModelsEqual(parentViewModel.Content,newViewModel) )
                {
                    parentViewModel.Content = newViewModel;
                }
            
                newViewModelTree.Add(newViewModel);
                parentViewModel = newViewModel;
                counter++;
            }

            _currentViewModelPack = newViewModelTree;
            Push(_currentViewModelPack.ToArray());
        }
        finally
        {
            _stackUpdateInProgress = false;
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
    
    private void Push(TViewModelBase[] stack)
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
            return _currentViewModelPack.ToArray();
        }

        // viewModel == null means offset is invalid
        // _historyIndex can be updated after this without further checks
        var viewModelPack = GetHistoryItem(offset);
        if (!viewModelPack.Any())
        {
            return viewModelPack;
        }

        if (viewModelPack.Any())
        {
            _historyIndex += offset;
            _currentViewModelPack = viewModelPack.ToList();
            if (viewModelPack.Length > 1)
            {
                var mainViewModel = viewModelPack.FirstOrDefault() ?? CreateViewModel(typeof(TMainViewModel));
                mainViewModel.Content = viewModelPack.ElementAt(1);
            }
        }

        return viewModelPack;
    }
    
    public TViewModelBase[] Back() => HasPrev ? Go(-1) : _currentViewModelPack.ToArray();
    
    public TViewModelBase[] Forward() => HasNext ? Go(1) : _currentViewModelPack.ToArray();
    
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
        UpdateCurrentViewModelStack(viewModelTypes);
        return _currentViewModelPack.ToArray();
    }
    
     
    
}