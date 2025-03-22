using Minifarm.Common.Enums;
using Minifarm.ScriptableObjects;

namespace Minifarm.Managers
{
    public interface IResourceManager
    {
        int GetResourceAmount(ResourceType type);
        bool HasResources(ResourceType type, int amount);
        void AddResource(ResourceType type, int amount);
        bool TryConsumeResource(ResourceType type, int amount);
        ResourceData GetResourceData(ResourceType type);
    }
}