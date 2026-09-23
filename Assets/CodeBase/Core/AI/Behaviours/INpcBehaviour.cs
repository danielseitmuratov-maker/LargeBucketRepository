using _Root._Scripts.Core.AI.Core;

namespace _Root._Scripts.Core.AI.Behaviours
{
    public interface INpcBehaviour
    {
        public int Priority { get; }
        public bool CanRun(NpcContext context);
        public void Enter(NpcContext context) { }
        public void Tick(NpcContext context) { }
        public  void Exit(NpcContext context) { }
    }
}