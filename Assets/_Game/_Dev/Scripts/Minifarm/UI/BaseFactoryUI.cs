using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minifarm.Factory;

namespace Minifarm.UI
{
    public class BaseFactoryUI : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI storageText;
        [SerializeField] protected TextMeshProUGUI productionTimeText;
        [SerializeField] protected Slider progressBar;
        [SerializeField] protected Image productIcon;

        protected BaseFactory currentFactory;
        protected CompositeDisposable disposables = new CompositeDisposable();

        public virtual void Initialize(BaseFactory factory)
        {
            disposables.Clear();

            currentFactory = factory;

            if (productIcon != null && factory.GetConfig().productIcon != null)
            {
                productIcon.sprite = factory.GetConfig().productIcon;
            }

            factory.StorageAmount
                .Subscribe(amount => UpdateStorageText(amount, factory.GetConfig().storageCapacity))
                .AddTo(disposables);

            factory.ProductionProgress
                .Subscribe(progress => UpdateProductionProgress(progress, factory.GetConfig().productionTime))
                .AddTo(disposables);
        }

        protected virtual void UpdateStorageText(int amount, int capacity)
        {
            storageText.text = $"{amount}/{capacity}";
        }

        protected virtual void UpdateProductionProgress(float progress, float productionTime)
        {
            progressBar.value = progress;
            int secondsRemaining = Mathf.CeilToInt((1 - progress) * productionTime);
            productionTimeText.text = secondsRemaining > 0 ? $"{secondsRemaining} sec" : "Idle";
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}