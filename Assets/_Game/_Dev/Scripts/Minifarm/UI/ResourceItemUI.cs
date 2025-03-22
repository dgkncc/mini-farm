using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using Minifarm.ScriptableObjects;
using Minifarm.Common.Enums;
using Minifarm.Managers;

namespace Minifarm.UI
{
    public class ResourceItemUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountText;
        
        private IDisposable subscription;

        public void Initialize(ResourceType type, IResourceManager resourceManager)
        {            
            ResourceData data = resourceManager.GetResourceData(type);
            if (data != null && icon != null)
            {
                icon.sprite = data.icon;
            }
            
            int initialAmount = resourceManager.GetResourceAmount(type);
            UpdateAmount(initialAmount);
            
            if (resourceManager is ResourceManager rm)
            {
                subscription = rm.GetResourceObservable(type)
                    .Subscribe(UpdateAmount);
            }
        }
        
        private void UpdateAmount(int amount)
        {
            if (amountText != null)
            {
                amountText.text = amount.ToString();
            }
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}