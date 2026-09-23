using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.Cursor
{
    public class CursorLocker : ICursorLocker
    {
        public bool IsLocked => _isLocked;

        private bool _isLocked;
        
        public void Lock()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            _isLocked = true;
        }

        public void Unlock()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            _isLocked = false;
        }

    }
}