using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter;

public class NestedHistoryRouter<TViewModelBase, TMainViewModel> 
    where TViewModelBase : ISimpleRoute<TViewModelBase>
    where TMainViewModel: TViewModelBase
{

    private int _historyIndex = -1;
    private List<Stack<TViewModelBase>> _history = new();
    private readonly uint _historyMaxSize = 100;
    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > 0;
    
    public IReadOnlyCollection<Stack<TViewModelBase>> History => _history.AsReadOnly();
    private Stack<TViewModelBase> _currentViewModelStack = new();
    
    protected readonly Func<Type, TViewModelBase> CreateViewModel;

    public NestedHistoryRouter( Func<Type, TViewModelBase> createViewModel)
    {
        CreateViewModel = createViewModel;
    }

    /*
    // TODO: implement history
    public void UpdateCurrentViewModel(params TViewModelBase[] viewModels)
    {
        // clone the original stack
        var viewModelTree = new Stack<TViewModelBase>(_currentViewModelStack.ToArray().Reverse());
        var newViewModelTree = new Stack<TViewModelBase>();
        var counter = 0;
        TViewModelBase lastViewModel = _mainViewModel;
        newViewModelTree.Push(_mainViewModel);
        foreach (var vm in viewModels)
        {
            // viewModels can be reused on the same level
            var newViewModel = viewModelTree.Count >= counter && AreViewModelsEqual(viewModelTree.ElementAt(counter), vm)
                ? viewModelTree.ElementAt(counter)
                : vm; // this has to be 
            
            if (!AreViewModelsEqual(lastViewModel.Content , newViewModel) )
            {
                lastViewModel.Content = newViewModel;
            }
            
            newViewModelTree.Push(newViewModel);
            counter++;
        }

        _currentViewModelStack = newViewModelTree;
    }
*/
    
    private static bool AreViewModelsEqual(TViewModelBase? first, TViewModelBase? second) => EqualityComparer<TViewModelBase?>.Default.Equals(first, second);

    
    private void UpdateCurrentViewModelStack(params Type[] viewModelTypes)
    {
        if (_currentViewModelStack.Count == 0)
        {
            _currentViewModelStack.Push(CreateViewModel(typeof(TMainViewModel)));
        }

        // clone the original stack
        var viewModelTree = new Stack<TViewModelBase>(_currentViewModelStack.ToArray().Reverse());
        TViewModelBase parentViewModel = viewModelTree.First();
        var newViewModelTree = new Stack<TViewModelBase>([parentViewModel]);
        var counter = 1; // start on 1 because MainViewModel is excluded in viewModelTypes
        foreach (var type in viewModelTypes)
        {
            // viewModels can be reused on the same level
            var newViewModel = viewModelTree.Count > counter && viewModelTree.ElementAt(counter).GetType() == type
                ? viewModelTree.ElementAt(counter)
                : CreateViewModel(type); // this has to be 
            
            if (!AreViewModelsEqual(parentViewModel.Content,newViewModel) )
            {
                parentViewModel.Content = newViewModel;
            }
            
            newViewModelTree.Push(newViewModel);
            counter++;
        }

        _currentViewModelStack = newViewModelTree;
    }
    
    public Stack<TViewModelBase> GetHistoryItem(int offset)
    {
        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return new();
        }
        return _history.ElementAt(newIndex);
    }
    
    public void Push(Stack<TViewModelBase> stack)
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
    
    public Stack<TViewModelBase> GoTo<T>() 
        where T : TViewModelBase => Goto(typeof(T));
    
    public Stack<TViewModelBase> GoTo<T1, T2>() 
        where T1 : TViewModelBase 
        where T2: TViewModelBase => Goto(typeof(T1), typeof(T2));

    public Stack<TViewModelBase> GoTo<T1, T2, T3>() 
        where T1 : TViewModelBase 
        where T2 : TViewModelBase 
        where T3: TViewModelBase => Goto(typeof(T1), typeof(T2), typeof(T3));
    
    public Stack<TViewModelBase> Goto(params Type[] viewModelTypes)
    {
        UpdateCurrentViewModelStack(viewModelTypes);
        return _currentViewModelStack;
    }
    
     
    
}