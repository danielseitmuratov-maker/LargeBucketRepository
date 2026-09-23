namespace _Root._Scripts.Infrastructure.Services.Cursor
{
    public interface ICursorLocker
    {
        void Lock();
        void Unlock();
        bool IsLocked { get; }
    }
}