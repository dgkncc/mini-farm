using UnityEngine;
namespace Minifarm.DependencyInjection
{
    public abstract class BaseInstaller : MonoBehaviour, IInstaller
    {
        public abstract void InstallBindings(DIContainer container);
    }
}