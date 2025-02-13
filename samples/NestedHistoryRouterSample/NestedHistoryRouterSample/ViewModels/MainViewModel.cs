using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using NestedHistoryRouterSample.ViewModels.Home;

namespace NestedHistoryRouterSample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
        router.GoTo<HomeViewModel,FirstSubViewModel>();
    }
}
