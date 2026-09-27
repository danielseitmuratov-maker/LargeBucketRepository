namespace CodeBase.Infrastructure.Services.Saves
{
    public interface ISaveLoadService
    {
        //SavesYG Data { get; }
        void Save();
        void ResetData();
        bool HasSave();
        void MarkDirty();
    }
}