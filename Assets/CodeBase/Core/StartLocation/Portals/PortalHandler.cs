using _Root._Scripts.Core.Character;
using _Root._Scripts.Core.Locations.Hostile;
using _Root._Scripts.Infrastructure.Services.Loaders;
using UnityEngine;

namespace _Root._Scripts.Core.StartLocation.Portals
{
    public class PortalHandler : MonoBehaviour
    {
        [SerializeField] private PortalType _type;
        
     ///   private ILoader<HostileLocationRoot> _hostileLocationLoader;

     //  public void Init(ILoader<HostileLocationRoot> hostileLocationLoader)
     //  { _hostileLocationLoader = hostileLocationLoader;
     //  }

     //  private void OnTriggerEnter(Collider othe    r)
     //  {
     //      if (other.TryGetComponent(out CharacterRoot characterRoot))
     //      {
     //          _hostileLocationLoader.Load(0);
     //      }
     //  }
    }

    public class PortalView : MonoBehaviour
    {
        
    }
}