# Avalonia.SimpleRouter
Cross platform router library mainly targeting AvaloniaUI. 

Since this is a dependency free implementation of a simple Router, you might also be able to use this with other UI Frameworks

# Install

📦 [NuGet](https://nuget.org/packages/Sandreas.Avalonia.SimpleRouter): `dotnet add package Sandreas.Avalonia.SimpleRouter`


# Features

- IoC / Dependency Injection support
- No dependencies
- Parameters / ViewModel Properties
- Routing history including `Back` and `Forward`
- Extendable and flexible


# API examples

All these API examples presume that you have a field called `_router`  of type `HistoryRouter<ViewModelBase>` in your ViewModel / Class. 
Usually this is achieved via Dependency Injection / IoC. See the full example below for more details.

Additionally, the examples use a `ViewLocator` class to map ViewModels to their according view classes.
This class once was part of the official Avalonia Template, but no longer is, so if you would like to use it,
you have to create it manually and add `<local:ViewLocator/>` to your `App.axaml`. 
Please ensure to change the namespace `ToneAudioPlayer` to match your project.

## ViewLocator
This is only required if you don't have the `ViewLocator`, see [issue #2](https://github.com/sandreas/Avalonia.SimpleRouter/issues/2).
<details>

```c#
// ToneAudioPlayer/ViewLocator.cs
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ToneAudioPlayer.ViewModels;

namespace ToneAudioPlayer;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        return new TextBlock { Text = name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
```

```xaml
<!-- MyApp/App.axaml -->
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:ToneAudioPlayer"
             x:Class="ToneAudioPlayer.App"
             RequestedThemeVariant="Default">
             <!-- "Default" ThemeVariant follows system theme variant. "Dark" or "Light" are other available options. -->

    <Application.DataTemplates>
        <local:ViewLocator/>
    </Application.DataTemplates>

    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```
</details>

## Examples

**Simple Transition**

Just change the visible view.
```c#
// Navigate to HomeView
_router.GoTo<HomeViewModel>();
```

**Transition with parameters**

The `Goto` method will return the destination viewModel, so you can change values. 
This is similar to route parameters but type safe and more flexible.
```c#
// navigate to "Settings", and set a property value of the destination viewModel
var settingsVm = _router.GoTo<SettingsViewModel>();
settingsVm.DefaultUsername = "root";
```

**Check history before transition**

You can check, if there is a routing history by using `HasNext` and `HasPrev` properties.
```c#
// check history if there is a forward option
if(_router.HasNext) {
    _router.Forward(); // navigate forward
}

// check history if there was a previous item
if(_router.HasPrev) {
    _router.Back(); // go back to last ViewModel
}
```

**Move in history by numeric value**
If you would like to navigate by a numeric value, this is also possible.
```c#
// go back two history items if possible, otherwise it will stay where you are
_router.Go(-2);
```

# Full Code Example

The following example includes `CommunityToolkit`, `DependencyInjection` and other helpful dependencies for starting a professional Avalonia App. 
This is not required but shows what this library can do in easy steps.

If you would like to see a fully working (but still incomplete) example as a project, check out [ToneAudioPlayer](https://github.com/sandreas/ToneAudioPlayer)

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