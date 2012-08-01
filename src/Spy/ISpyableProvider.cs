namespace Spy
{
    public interface ISpyableProvider
    {
        string Name { get; }
        string Description { get; }
        object SpyableObject { get; }
    }
}