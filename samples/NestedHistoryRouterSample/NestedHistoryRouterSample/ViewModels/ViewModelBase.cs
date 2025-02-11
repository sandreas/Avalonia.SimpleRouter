using Avalonia.SimpleRouter.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NestedHistoryRouterSample.ViewModels;

public abstract partial class ViewModelBase : ObservableObject, ISimpleRoute<ViewModelBase>
{
    [ObservableProperty] private ViewModelBase? _content;
    // public ViewModelBase? Content { get; set; }
}
