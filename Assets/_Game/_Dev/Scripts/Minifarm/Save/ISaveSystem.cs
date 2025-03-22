namespace Minifarm.Save
{
    public interface ISaveSystem
    {
        void SaveGame();
        void LoadGame();
        void RegisterAutoSave();
    }
}