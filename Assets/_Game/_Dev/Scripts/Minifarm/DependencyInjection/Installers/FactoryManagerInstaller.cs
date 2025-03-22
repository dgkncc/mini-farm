using Minifarm.Factory;
using Minifarm.Managers;

namespace Minifarm.DependencyInjection.Installers
{
    public class FactoryManagerInstaller : BaseInstaller
    {
        public override void InstallBindings(DIContainer container)
        {
            var factoryManager = new FactoryManager();
            container.Bind<IFactoryManager>(factoryManager);
            container.Bind<FactoryManager>(factoryManager);

            foreach (var factory in FindObjectsOfType<BaseFactory>())
            {
                factoryManager.RegisterFactory(factory);
            }
        }
    }
}