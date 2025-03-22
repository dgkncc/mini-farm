using UnityEngine;
using Minifarm._Core.EventService;
using System.Collections.Generic;
using Minifarm.Common.Events;
using Minifarm.Factory.Factories;
using Minifarm.DependencyInjection;
using Minifarm.Managers;

namespace Minifarm.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private HayFactoryUI hayFactoryUIPrefab;
        [SerializeField] private FactoryInfoUI factoryInfoUIPrefab;
        [SerializeField] private Transform uiContainer;

        private Dictionary<string, BaseFactoryUI> factoryUIs = new Dictionary<string, BaseFactoryUI>();
        private FactoryInfoUI sharedFactoryInfoUI;
        private string currentOpenFactoryId;

        private void Start()
        {
            GameEventService.On<FactoryClickedEvent>(OnFactoryClicked);
            GameEventService.On<EmptyClickEvent>(OnEmptyClick);

            sharedFactoryInfoUI = Instantiate(factoryInfoUIPrefab, uiContainer);
            sharedFactoryInfoUI.gameObject.SetActive(false);

            var factoryManager = GameInstaller.Instance.Resolve<IFactoryManager>();
            foreach (var factory in factoryManager.GetFactories())
            {
                if (factory is HayFactory)
                {
                    var hayUI = Instantiate(hayFactoryUIPrefab, uiContainer);
                    hayUI.Initialize(factory);
                    factoryUIs.Add(factory.GetFactoryId(), hayUI);
                }
            }
        }

        private void OnDestroy()
        {
            GameEventService.Off<FactoryClickedEvent>(OnFactoryClicked);
            GameEventService.Off<EmptyClickEvent>(OnEmptyClick);
        }

        private void OnFactoryClicked(FactoryClickedEvent e)
        {
            var factory = e.Factory;

            if (factory is HayFactory)
            {
                sharedFactoryInfoUI.gameObject.SetActive(false);
                currentOpenFactoryId = null;
                return;
            }

            sharedFactoryInfoUI.gameObject.SetActive(true);
            sharedFactoryInfoUI.Initialize(factory);
            currentOpenFactoryId = e.FactoryId;
        }

        private void OnEmptyClick(EmptyClickEvent e)
        {
            sharedFactoryInfoUI.gameObject.SetActive(false);
            currentOpenFactoryId = null;
        }
    }
}