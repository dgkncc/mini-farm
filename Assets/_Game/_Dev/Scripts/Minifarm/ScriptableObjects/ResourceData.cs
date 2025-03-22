using Minifarm.Common.Enums;
using UnityEngine;

namespace Minifarm.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Game/Resources/ResourceData")]
    public class ResourceData : ScriptableObject
    {
        [Tooltip("It should match the ResourceType enum value")]
        public string resourceName;
        public ResourceType resourceType;
        public Sprite icon;
    }
}