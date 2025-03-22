using System;
using UniRx;
using UnityEngine;
using Minifarm._Core.EventService;
using Minifarm.Common.Events;
using Minifarm.Common.Enums;
using Minifarm.DependencyInjection;
using Minifarm.Managers;


namespace Minifarm.UI
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private ResourceItemUI resourceItemPrefab;
        [SerializeField] private Transform resourceContainer;

        private IResourceManager resourceManager;
        private CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            resourceManager = GameInstaller.Instance.Resolve<IResourceManager>();

            GameEventService.On<ResourceChangeEvent>(OnResourceChanged);

            InitializeUI();
        }

        private void OnDestroy()
        {
            GameEventService.Off<ResourceChangeEvent>(OnResourceChanged);
            disposables.Dispose();
        }

        private void InitializeUI()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                var resourceItem = Instantiate(resourceItemPrefab, resourceContainer);
                resourceItem.name = $"Resource_{type}";
                resourceItem.Initialize(type, resourceManager);
            }
        }

        private void OnResourceChanged(ResourceChangeEvent e)
        {
        }
    }
}