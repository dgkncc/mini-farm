using Minifarm.DependencyInjection;
using Minifarm.Managers;
using Minifarm.Save;

namespace Minifarm.DependencyInjection.Installers
{
    public class SaveSystemInstaller : BaseInstaller
    {
        public override void InstallBindings(DIContainer container)
        {
            var factoryManager = container.Resolve<IFactoryManager>();
            var resourceManager = container.Resolve<ResourceManager>();

            var saveSystem = new SaveSystem(factoryManager, resourceManager);
            container.Bind<ISaveSystem>(saveSystem);


        }
    }
}