namespace Avalonia.SimpleRouter.Interfaces;

public interface ISimpleRoute<T>
{
    public T? Content { get; set; }
}