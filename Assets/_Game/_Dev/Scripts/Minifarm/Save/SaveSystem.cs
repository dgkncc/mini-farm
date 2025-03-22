using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Minifarm.Managers;
using Minifarm.Common.Enums;

namespace Minifarm.Save
{
    public class SaveSystem : ISaveSystem
    {
        private const string SAVE_KEY = "GameSaveData";
        private const int AUTO_SAVE_INTERVAL = 5;

        private IFactoryManager factoryManager;
        private ResourceManager resourceManager;
        private bool isAutoSaving = false;

        public SaveSystem(IFactoryManager factoryManager, ResourceManager resourceManager)
        {
            this.factoryManager = factoryManager;
            this.resourceManager = resourceManager;
            DebugManager.Instance.Log("SaveSystem initialized");
        }

        public void SaveGame()
        {
            DebugManager.Instance.Log("Starting SaveGame process");

            GameSaveData saveData = new GameSaveData
            {
                LastSaveTime = DateTime.Now.Ticks,
                Factories = new List<FactorySaveData>(),
                Resources = new List<ResourceSaveData>()
            };

            var factories = factoryManager.GetFactories();
            DebugManager.Instance.Log($"Found {factories.Length} factories to save");

            foreach (var factory in factories)
            {
                var factorySaveData = factory.GetSaveData();
                saveData.Factories.Add(factorySaveData);
                DebugManager.Instance.Log($"Saved factory: {factorySaveData.ConfigId} with storage: {factorySaveData.StorageAmount}, queue: {factorySaveData.QueuedAmount}");
            }

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                int amount = resourceManager.GetResourceAmount(type);
                saveData.Resources.Add(new ResourceSaveData
                {
                    Type = type,
                    Amount = amount
                });
                DebugManager.Instance.Log($"Saved resource: {type} with amount: {amount}");
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
            DebugManager.Instance.Log("Game saved successfully!");
        }

        public void LoadGame()
        {
            DebugManager.Instance.Log("Starting LoadGame process");

            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                DebugManager.Instance.Log("No save data found. Starting new game.", DebugManager.LogLevel.Warning);
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                DebugManager.Instance.Log($"Loaded save data: {json}");

                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                if (saveData == null)
                {
                    DebugManager.Instance.Log("Failed to parse save data!", DebugManager.LogLevel.Error);
                    return;
                }

                DebugManager.Instance.Log($"Save contains {saveData.Resources.Count} resources and {saveData.Factories.Count} factories");

                foreach (var resourceData in saveData.Resources)
                {
                    resourceManager.SetResourceAmount(resourceData.Type, resourceData.Amount);
                    DebugManager.Instance.Log($"Loaded resource: {resourceData.Type} with amount: {resourceData.Amount}");
                }

                foreach (var factoryData in saveData.Factories)
                {
                    DebugManager.Instance.Log($"Trying to load factory with ID: {factoryData.ConfigId}");

                    var factory = factoryManager.GetFactoryById(factoryData.ConfigId);
                    if (factory != null)
                    {
                        factory.LoadSaveData(factoryData);
                        DebugManager.Instance.Log($"Successfully loaded factory: {factoryData.ConfigId}");
                    }
                    else
                    {
                        DebugManager.Instance.Log($"Factory with ID {factoryData.ConfigId} not found!", DebugManager.LogLevel.Warning);
                    }
                }

                DebugManager.Instance.Log("Game loaded successfully!");
            }
            catch (Exception ex)
            {
                DebugManager.Instance.Log($"Error loading game: {ex.Message}\n{ex.StackTrace}", DebugManager.LogLevel.Error);
            }
        }

        public void RegisterAutoSave()
        {
            if (!isAutoSaving)
            {
                isAutoSaving = true;
                DebugManager.Instance.Log("Auto-save registered");
                AutoSaveAsync().Forget();
            }
        }

        private async UniTask AutoSaveAsync()
        {
            DebugManager.Instance.Log($"Auto-save started with interval: {AUTO_SAVE_INTERVAL} seconds");
            while (isAutoSaving)
            {
                await UniTask.Delay(AUTO_SAVE_INTERVAL * 1000);
                DebugManager.Instance.Log("Auto-save triggered");
                SaveGame();
            }
        }
    }
}