using Avalonia.SimpleRouter;

namespace NestedHistoryRouterSample.ViewModels.Home;

public partial class FirstSubViewModel : ViewModelBase
{


        // router.CurrentViewModelChanged += viewModel => Content = viewModel;
    public FirstSubViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router)
    {
    }

}