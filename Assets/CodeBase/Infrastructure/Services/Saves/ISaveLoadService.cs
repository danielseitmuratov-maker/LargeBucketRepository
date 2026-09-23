using YG;

namespace _Root._Scripts.Infrastructure.Services.Saves
{
    public interface ISaveLoadService
    {
        SavesYG Data { get; }
        void Save();
        void ResetData();
        bool HasSave();
        void MarkDirty();
    }
}