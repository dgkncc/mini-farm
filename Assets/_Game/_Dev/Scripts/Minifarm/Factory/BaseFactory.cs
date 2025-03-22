using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minifarm._Core.EventService;
using Minifarm.Common.Enums;
using Minifarm.Common.Events;
using Minifarm.DependencyInjection;
using Minifarm.Managers;
using Minifarm.Save;
using Minifarm.ScriptableObjects;
using UniRx;
using UnityEngine;

namespace Minifarm.Factory
{
    public abstract class BaseFactory : MonoBehaviour
    {
        [SerializeField] protected FactoryConfig config;

        protected IResourceManager resourceManager;
        protected ReactiveProperty<int> storageAmount = new ReactiveProperty<int>(0);
        protected ReactiveProperty<int> queuedAmount = new ReactiveProperty<int>(0);
        protected ReactiveProperty<float> productionProgress = new ReactiveProperty<float>(0);
        protected bool isProducing = false;
        protected CancellationTokenSource productionCancellation;
        protected bool isSelected = false;

        protected virtual void Start()
        {
            if (config == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(config.persistentId))
            {
            }

            resourceManager = GameInstaller.Instance.Resolve<IResourceManager>();

            if ((queuedAmount.Value > 0 || !config.hasQueue) && CanProduceMore())
            {
                StartProductionAsync().Forget();
            }
        }

        public void OnClick()
        {
            if (isSelected || !config.hasQueue)
                HarvestStorage();
            else
                isSelected = true;

            GameEventService.Fire(new FactoryClickedEvent
            {
                FactoryId = config.persistentId,
                Factory = this
            });
        }

        public virtual void HarvestStorage()
        {
            if (storageAmount.Value > 0)
            {
                var resourceType = GetResourceType();
                resourceManager.AddResource(resourceType, storageAmount.Value);
                storageAmount.Value = 0;

                if (!isProducing && (queuedAmount.Value > 0 || !config.hasQueue))
                {
                    StartProductionAsync().Forget();
                }
            }
        }

        protected abstract ResourceType GetResourceType();

        public virtual bool CanProduceMore()
        {
            return storageAmount.Value < config.storageCapacity;
        }

        public virtual bool HasRequiredResources()
        {
            if (config.requiredResources == null || config.requiredResources.Length == 0)
            {
                return true;
            }

            for (int i = 0; i < config.requiredResources.Length; i++)
            {
                ResourceType requiredType = config.requiredResources[i].resourceType;
                int requiredAmount = config.requiredResourceAmounts[i];

                if (!resourceManager.HasResources(requiredType, requiredAmount))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void ConsumeRequiredResources()
        {
            if (config.requiredResources == null || config.requiredResources.Length == 0)
            {
                return;
            }

            for (int i = 0; i < config.requiredResources.Length; i++)
            {
                ResourceType requiredType = config.requiredResources[i].resourceType;
                int requiredAmount = config.requiredResourceAmounts[i];

                resourceManager.TryConsumeResource(requiredType, requiredAmount);
            }
        }

        public virtual void QueueProduction(int amount)
        {
            if (!config.hasQueue) return;

            int maxAdditional = config.storageCapacity - queuedAmount.Value - storageAmount.Value;
            int toAdd = Mathf.Min(amount, maxAdditional);

            if (toAdd > 0)
            {
                queuedAmount.Value += toAdd;

                if (!isProducing)
                {
                    StartProductionAsync().Forget();
                }
            }
        }

        public virtual void DequeueProduction(int amount)
        {
            if (!config.hasQueue) return;

            int toRemove = Mathf.Min(amount, queuedAmount.Value);
            queuedAmount.Value -= toRemove;
        }

        protected async UniTask StartProductionAsync()
        {
            if (isProducing) return;

            isProducing = true;
            productionCancellation = new CancellationTokenSource();

            try
            {
                while (CanProduceMore() && (queuedAmount.Value > 0 || !config.hasQueue))
                {
                    if (config.hasQueue && !HasRequiredResources())
                    {
                        await UniTask.Delay(1000, cancellationToken: productionCancellation.Token);
                        continue;
                    }

                    ConsumeRequiredResources();

                    if (config.hasQueue)
                    {
                        queuedAmount.Value--;
                    }

                    float progress = 0;
                    float startTime = Time.time;
                    float endTime = startTime + config.productionTime;

                    while (Time.time < endTime)
                    {
                        progress = (Time.time - startTime) / config.productionTime;
                        productionProgress.Value = progress;
                        await UniTask.Yield(productionCancellation.Token);
                    }

                    productionProgress.Value = 0;
                    storageAmount.Value++;

                    GameEventService.Fire(new FactoryProductionEvent
                    {
                        FactoryId = config.persistentId,
                        ResourceType = GetResourceType(),
                        Amount = 1
                    });

                    if (!config.hasQueue && storageAmount.Value >= config.storageCapacity)
                    {
                        isProducing = false;
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                isProducing = false;
            }
        }

        public string GetFactoryId() => config.persistentId;
        public FactoryConfig GetConfig() => config;
        public IReadOnlyReactiveProperty<int> StorageAmount => storageAmount;
        public IReadOnlyReactiveProperty<int> QueuedAmount => queuedAmount;
        public IReadOnlyReactiveProperty<float> ProductionProgress => productionProgress;

        public virtual FactorySaveData GetSaveData()
        {
            return new FactorySaveData
            {
                ConfigId = config.persistentId,
                StorageAmount = storageAmount.Value,
                QueuedAmount = queuedAmount.Value,
                ProductionProgress = productionProgress.Value,
                LastProductionTime = DateTime.Now.Ticks
            };
        }

        public virtual void StopProduction()
        {
            if (isProducing && productionCancellation != null)
            {
                productionCancellation.Cancel();
                productionCancellation.Dispose();
                productionCancellation = null;
            }

            isProducing = false;
            productionProgress.Value = 0;

        }

        public virtual void LoadSaveData(FactorySaveData data)
        {
            storageAmount.Value = data.StorageAmount;
            queuedAmount.Value = data.QueuedAmount;
            productionProgress.Value = data.ProductionProgress;

            CalculateOfflineProduction(data.LastProductionTime);

            if (!CanProduceMore())
            {
                StopProduction();
            }
            else if (queuedAmount.Value > 0 || !config.hasQueue)
            {
                StartProductionAsync().Forget();
            }
        }

        public void ChangeSelectedState(bool state)
        {
            isSelected = state;
        }

        protected virtual void CalculateOfflineProduction(long lastProductionTimeTicks)
        {
            DateTime lastProductionTime = new DateTime(lastProductionTimeTicks);
            TimeSpan offlineTime = DateTime.Now - lastProductionTime;
            float offlineSeconds = (float)offlineTime.TotalSeconds;

            if (productionProgress.Value > 0)
            {
                float productionTimeLeft = config.productionTime * (1 - productionProgress.Value);

                if (offlineSeconds >= productionTimeLeft)
                {
                    storageAmount.Value++;
                    offlineSeconds -= productionTimeLeft;
                    productionProgress.Value = 0;


                    GameEventService.Fire(new FactoryProductionEvent
                    {
                        FactoryId = config.persistentId,
                        ResourceType = GetResourceType(),
                        Amount = 1
                    });
                }
                else
                {
                    productionProgress.Value += offlineSeconds / config.productionTime;
                    return;
                }
            }

            if (offlineSeconds > 0 && (queuedAmount.Value > 0 || !config.hasQueue))
            {
                int itemsToAdd = 0;
                int possibleCompletedItems = (int)(offlineSeconds / config.productionTime);
                int storageSpace = config.storageCapacity - storageAmount.Value;

                DebugManager.Instance.Log($"Can process up to {possibleCompletedItems} more items, storage space: {storageSpace}");

                if (possibleCompletedItems > 0 && storageSpace > 0)
                {
                    if (config.hasQueue)
                    {
                        int resourceLimit = 0;

                        if (config.requiredResources != null && config.requiredResources.Length > 0)
                        {
                            resourceLimit = int.MaxValue;

                            for (int i = 0; i < config.requiredResources.Length; i++)
                            {
                                ResourceType requiredType = config.requiredResources[i].resourceType;
                                int requiredAmountPerItem = config.requiredResourceAmounts[i];
                                int availableAmount = resourceManager.GetResourceAmount(requiredType);

                                if (requiredAmountPerItem > 0)
                                {
                                    int possibleItems = availableAmount / requiredAmountPerItem;
                                    resourceLimit = Mathf.Min(resourceLimit, possibleItems);
                                }
                            }

                        }
                        else
                        {
                            resourceLimit = int.MaxValue;
                        }

                        itemsToAdd = Mathf.Min(
                            possibleCompletedItems,
                            queuedAmount.Value,
                            storageSpace,
                            resourceLimit
                        );

                        if (itemsToAdd > 0 && config.requiredResources != null && config.requiredResources.Length > 0)
                        {
                            for (int i = 0; i < config.requiredResources.Length; i++)
                            {
                                ResourceType requiredType = config.requiredResources[i].resourceType;
                                int requiredAmountTotal = config.requiredResourceAmounts[i] * itemsToAdd;

                                resourceManager.TryConsumeResource(requiredType, requiredAmountTotal);
                            }
                        }

                        queuedAmount.Value -= itemsToAdd;
                    }
                    else
                    {
                        itemsToAdd = Mathf.Min(possibleCompletedItems, storageSpace);
                    }

                    if (itemsToAdd > 0)
                    {
                        storageAmount.Value += itemsToAdd;


                        GameEventService.Fire(new FactoryProductionEvent
                        {
                            FactoryId = config.persistentId,
                            ResourceType = GetResourceType(),
                            Amount = itemsToAdd
                        });

                        float remainingSeconds = offlineSeconds - (itemsToAdd * config.productionTime);
                        if (remainingSeconds > 0 && (queuedAmount.Value > 0 || !config.hasQueue) && storageAmount.Value < config.storageCapacity)
                        {
                            bool hasResourcesForNext = !config.hasQueue || !HasRequiredResources();

                            if (hasResourcesForNext)
                            {
                                productionProgress.Value = remainingSeconds / config.productionTime;
                            }
                        }
                    }
                }
            }

        }


    }
}