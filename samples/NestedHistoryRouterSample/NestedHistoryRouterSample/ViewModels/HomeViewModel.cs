

using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NestedHistoryRouterSample.ViewModels;

public partial class HomeViewModel : ViewModelBase
{

    // [ObservableProperty] private string _etwas = "hallo";
    public string Etwas { get; set; } = "hallo";
        // router.CurrentViewModelChanged += viewModel => Content = viewModel;
    public HomeViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
        
    }

}