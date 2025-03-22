using System;
using System.Collections.Generic;
using Minifarm._Core.EventService;
using Minifarm.Common.Enums;
using Minifarm.Common.Events;
using Minifarm.ScriptableObjects;
using UniRx;
using UnityEngine;

namespace Minifarm.Managers
{
    public class ResourceManager : IResourceManager
    {
        private Dictionary<ResourceType, ReactiveProperty<int>> resources = new Dictionary<ResourceType, ReactiveProperty<int>>();
        private Dictionary<ResourceType, ResourceData> resourceDataMap = new Dictionary<ResourceType, ResourceData>();

        public ResourceManager(List<ResourceData> resourceDataList)
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                resources[type] = new ReactiveProperty<int>(0);

                resources[type].Subscribe(amount =>
                {
                    GameEventService.Fire(new ResourceChangeEvent
                    {
                        ResourceType = type,
                        Amount = amount,
                        TotalAmount = amount
                    });
                });
            }

            foreach (var data in resourceDataList)
            {
                ResourceType type;
                if (Enum.TryParse(data.resourceName, true, out type))
                {
                    resourceDataMap[type] = data;
                }
                else
                {
                    DebugManager.Instance.Log($"Could not map ResourceData '{data.resourceName}' to any ResourceType",level: DebugManager.LogLevel.Warning);
                }
            }
        }

        public int GetResourceAmount(ResourceType type)
        {
            return resources[type].Value;
        }

        public bool HasResources(ResourceType type, int amount)
        {
            return resources[type].Value >= amount;
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (amount <= 0) return;
            resources[type].Value += amount;
        }

        public bool TryConsumeResource(ResourceType type, int amount)
        {
            if (amount <= 0) return true;

            if (resources[type].Value >= amount)
            {
                resources[type].Value -= amount;
                return true;
            }
            return false;
        }

        public ResourceData GetResourceData(ResourceType type)
        {
            if (resourceDataMap.TryGetValue(type, out ResourceData data))
            {
                return data;
            }
            return null;
        }

        public ReactiveProperty<int> GetResourceObservable(ResourceType type)
        {
            return resources[type];
        }

        public void SetResourceAmount(ResourceType type, int amount)
        {
            resources[type].Value = amount;
        }
    }
}