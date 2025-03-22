using UnityEngine;
using System;

namespace Minifarm.ScriptableObjects
{
    [CreateAssetMenu(fileName = "FactoryConfig", menuName = "Game/Factory/FactoryConfig")]
    public class FactoryConfig : ScriptableObject
    {
        [Header("Basic Settings")]
        public string factoryName;
        public string persistentId;
        
        [Header("Visual Elements")]
        public Sprite factoryIcon;
        public Sprite productIcon;
        
        [Header("Production Settings")]
        public ResourceData producedResource;
        public ResourceData[] requiredResources;
        public int[] requiredResourceAmounts;
        public float productionTime;
        public int storageCapacity;
        public bool hasQueue;
        public bool alwaysShowUI;
        
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(persistentId))
            {
                persistentId = Guid.NewGuid().ToString();
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
    }
}