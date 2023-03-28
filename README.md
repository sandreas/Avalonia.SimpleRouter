# Avalonia.SimpleRouter
Cross platform view (model) router library for AvaloniaUI

# Code Example:

```c#
var services = new ServiceCollection();
// ...        
services.AddSingleton<HistoryRouter<ViewModelBase>>(s => new HistoryRouter<ViewModelBase>((Type t) => s.GetRequiredService(t));
// ...        


var router = services.GetService<HistoryRouter<ViewModelBase>>();

router.GoTo<HomeViewModel>();
router.GoTo<SettingsViewModel>();

router.Back();
router.Forward();
router.Go(-2);
```