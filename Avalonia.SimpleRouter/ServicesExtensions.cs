using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.SimpleRouter;

public static class ServicesExtensions
{
    public static void UseHistoryRouter<T>(this IServiceCollection services) where T : class
    {
        services.AddSingleton<HistoryRouter<T>>(s => new HistoryRouter<T>(t => (T)s.GetRequiredService(t)));
    }
}