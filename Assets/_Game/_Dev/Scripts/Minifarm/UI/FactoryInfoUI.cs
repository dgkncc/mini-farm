using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minifarm.Factory;

namespace Minifarm.UI
{
    public class FactoryInfoUI : BaseFactoryUI
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Button queueInButton;
        [SerializeField] private Button queueOutButton;
        [SerializeField] private TextMeshProUGUI queueText;
        [SerializeField] private GameObject queuePanel;

        public override void Initialize(BaseFactory factory)
        {
            base.Initialize(factory);

            queuePanel.SetActive(factory.GetConfig().hasQueue);

            var screenPoint = Camera.main.WorldToScreenPoint(factory.transform.position);
            rectTransform.position = screenPoint;

            if (factory.GetConfig().hasQueue)
            {
                factory.QueuedAmount
                    .Subscribe(amount => UpdateQueueText(amount, factory.GetConfig().storageCapacity))
                    .AddTo(disposables);

                queueInButton.onClick.AsObservable()
                    .Subscribe(_ => factory.QueueProduction(1))
                    .AddTo(disposables);

                queueOutButton.onClick.AsObservable()
                    .Subscribe(_ => factory.DequeueProduction(1))
                    .AddTo(disposables);
            }
        }

        private void UpdateQueueText(int amount, int capacity)
        {
            queueText.text = $"{amount}/{capacity}";
        }
    }
}