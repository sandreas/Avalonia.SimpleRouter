using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using NestedHistoryRouterSample.ViewModels.Home;

namespace NestedHistoryRouterSample.ViewModels;

public class MainViewModel : ViewModelBase
{
    // [ObservableProperty] private ViewModelBase? _content = null;

    public MainViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
        router.GoTo<HomeViewModel,FirstSubViewModel>();
    }
}
