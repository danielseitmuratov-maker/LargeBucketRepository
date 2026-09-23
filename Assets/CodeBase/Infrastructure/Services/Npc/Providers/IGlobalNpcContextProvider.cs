using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Infrastructure.Services.Npc.Providers
{
    public interface IGlobalNpcContextProvider
    {
        GlobalNpcContext GetGlobalContext();
        void StartUpdateContext();
        void StopUpdateContext();
    }
}