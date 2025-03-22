using Minifarm.UI;

namespace Minifarm.DependencyInjection.Installers
{
    public class UIManagerInstaller : BaseInstaller
    {
        public override void InstallBindings(DIContainer container)
        {
            var uiManager = FindObjectOfType<UIManager>();
            container.Bind<UIManager>(uiManager);
        }
    }
}