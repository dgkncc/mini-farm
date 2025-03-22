using Minifarm._Core.Common;
using Minifarm.DependencyInjection.Installers;
using UnityEngine;

namespace Minifarm.DependencyInjection
{
    public class GameInstaller : SingleMonoBehaviour<GameInstaller>, IInstaller
    {
        [SerializeField] private ResourceManagerInstaller resourceManagerInstaller;
        [SerializeField] private FactoryManagerInstaller factoryManagerInstaller;
        [SerializeField] private SaveSystemInstaller saveSystemInstaller;
        [SerializeField] private UIManagerInstaller uiManagerInstaller;

        private DIContainer container;

        protected override void Awake()
        {
            base.Awake();
            container = new DIContainer();

            InstallBindings(container);
            resourceManagerInstaller.InstallBindings(container);
            factoryManagerInstaller.InstallBindings(container);
            saveSystemInstaller.InstallBindings(container);
            uiManagerInstaller.InstallBindings(container);
        }

        public void InstallBindings(DIContainer container)
        {
            container.Bind<DIContainer>(container);
            container.Bind<GameInstaller>(this);
        }

        public T Resolve<T>() where T : class
        {
            return container.Resolve<T>();
        }
    }
}