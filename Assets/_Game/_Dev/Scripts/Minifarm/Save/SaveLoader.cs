using Minifarm.DependencyInjection;
using UnityEngine;

namespace Minifarm.Save
{
    public class SaveLoader : MonoBehaviour
    {
        ISaveSystem saveSystem;

        private void Start()
        {
            saveSystem = GameInstaller.Instance.Resolve<ISaveSystem>();

            saveSystem.LoadGame();
            saveSystem.RegisterAutoSave();
        }
    }
}