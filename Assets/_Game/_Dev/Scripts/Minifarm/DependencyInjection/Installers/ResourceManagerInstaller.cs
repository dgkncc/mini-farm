using System.Collections.Generic;
using Minifarm.Managers;
using Minifarm.ScriptableObjects;
using UnityEngine;

namespace Minifarm.DependencyInjection.Installers
{
    public class ResourceManagerInstaller : BaseInstaller
    {
        [SerializeField] private List<ResourceData> resources;

        public override void InstallBindings(DIContainer container)
        {
            var resourceManager = new ResourceManager(resources);
            container.Bind<IResourceManager>(resourceManager);
            container.Bind<ResourceManager>(resourceManager);
        }
    }
}