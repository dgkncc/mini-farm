using Minifarm.DependencyInjection;
using Minifarm.Save;
using UnityEngine;

namespace Minifarm.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameInstaller gameInstaller;

        private void Start()
        {
            var saveSystem = gameInstaller.Resolve<ISaveSystem>();

            Application.quitting += OnApplicationQuit;
            Application.focusChanged += OnApplicationFocusChanged;
        }

        private void OnApplicationQuit()
        {
            var saveSystem = gameInstaller.Resolve<ISaveSystem>();
            //saveSystem.SaveGame();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
            {
                var saveSystem = gameInstaller.Resolve<ISaveSystem>();
                //saveSystem.SaveGame();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                var saveSystem = gameInstaller.Resolve<ISaveSystem>();
                //saveSystem.SaveGame();
            }
        }
    }
}