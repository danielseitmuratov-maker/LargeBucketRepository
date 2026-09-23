namespace _Root._Scripts.Infrastructure.Services.Loaders
{
    public interface IAutoLoader<T> : ILoader<T> where T : new()
    {
        void StartAutoLoad();
        void StartAutoUnload();
    }
}