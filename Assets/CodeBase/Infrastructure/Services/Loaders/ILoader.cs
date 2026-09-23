using System;

namespace _Root._Scripts.Infrastructure.Services.Loaders
{
    public interface ILoader<T> where T : new()
    {
        event Action Started;
        event Action<T,int> Loaded;
        event Action<T> UnLoaded;

        T Load(int id);
        void Unload(T unLoaded);   
    }
}