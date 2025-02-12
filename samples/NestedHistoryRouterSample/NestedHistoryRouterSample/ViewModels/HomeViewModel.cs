

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NestedHistoryRouterSample.ViewModels.Home;

namespace NestedHistoryRouterSample.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

    [ObservableProperty] private List<string> _breadCrumbItems = new();

    public HomeViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
        _router = router;
        _router.CurrentViewModelsChanged += (viewModelTree) => UpdateBreadCrumb(viewModelTree); 
    }

    private void UpdateBreadCrumb(ViewModelBase[] viewModelTree)
    {
        List<string> breadCrumbItems = [];
        var lastItem = viewModelTree.Last();
        foreach (var vm in viewModelTree)
        {
            breadCrumbItems.Add(vm.GetType().Name.Replace("ViewModel", ""));
            if (vm != lastItem)
            {
                breadCrumbItems.Add(" » ");
            }
        }
        BreadCrumbItems = breadCrumbItems;
    }

    [RelayCommand]
    private void NavigateTo(string destination)
    {
        switch (destination)
        {
            case "SecondSubView":
                _router.GoTo<HomeViewModel, SecondSubViewModel>();
                break;
            case "ThirdSubView":
                _router.GoTo<HomeViewModel, ThirdSubViewModel>();
                break;
            case "Back":
                _router.Back();
                break;
            case "Forward":
                _router.Forward();
                break; 
            default:
                _router.GoTo<HomeViewModel, FirstSubViewModel>();
                break;
        }
    }
    
    
}