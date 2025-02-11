

using System.Diagnostics;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NestedHistoryRouterSample.ViewModels.Home;

namespace NestedHistoryRouterSample.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;


    public HomeViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
        _router = router;
    }

    [RelayCommand]
    private void NavigateTo(string destination)
    {
        switch (destination)
        {
            case "SecondSubView":
                _router.GoTo<HomeViewModel, SecondSubViewModel>();
                break;
            default:
                _router.GoTo<HomeViewModel, FirstSubViewModel>();
                break;
        }
    }
    
    
}