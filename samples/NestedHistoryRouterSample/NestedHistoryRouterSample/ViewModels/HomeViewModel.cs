

using Avalonia.SimpleRouter;

namespace NestedHistoryRouterSample.ViewModels;

public partial class HomeViewModel : ViewModelBase
{


        // router.CurrentViewModelChanged += viewModel => Content = viewModel;
    public HomeViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
    }

}