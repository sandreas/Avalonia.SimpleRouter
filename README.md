# Avalonia.SimpleRouter
Cross platform view (model) router library for AvaloniaUI

# Code Example:

```c#
// App.axaml.cs
public partial class App : Application
{
    // ...
    
    public override void OnFrameworkInitializationCompleted()
    {
        // In this example we use Microsoft DependencyInjection (instead of ReactiveUI / Splat)
        // Splat would also work, just use the according methods
        IServiceProvider services = ConfigureServices();
        var mainViewModel = services.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        // Add the HistoryRouter as a service
        services.AddSingleton<HistoryRouter<ViewModelBase>>(s => new HistoryRouter<ViewModelBase>(t => (ViewModelBase)s.GetRequiredService(t)));

        // Add the ViewModels as a service (Main as singleton, others as transient)
        services.AddSingleton<MainViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        return services.BuildServiceProvider();
    }
}
```

```xaml
<!-- Views/MainView.axaml-->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:viewModels="clr-namespace:ToneAudioPlayer.ViewModels"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="ToneAudioPlayer.Views.MainView"
             x:DataType="viewModels:MainViewModel">
    <ContentControl Content="{Binding Content}"></ContentControl>
</UserControl>
```

```c#
// ViewModels/MainViewModel.cs
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ToneAudioPlayer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
       
    [ObservableProperty]
    private ViewModelBase _content = default!;

    public MainViewModel(HistoryRouter<ViewModelBase> router)
    {
        // register route changed event to set content to viewModel, whenever 
        // a route changes
        router.CurrentViewModelChanged += viewModel => Content = viewModel;
        
        // change to HomeView 
        router.GoTo<HomeViewModel>();
    }
}
```




```c#
// more API method examples
var settingsVm = router.GoTo<SettingsViewModel>();
settingsVm.DefaultUsername = "root";

if(router.HasNext) {
    router.Forward(); // go forward to 
}
if(router.HasPrev) {
    router.Back(); // go back to last ViewModel
}

router.Go(-2); // go back two routes if possible, otherwise stay where you are
```
