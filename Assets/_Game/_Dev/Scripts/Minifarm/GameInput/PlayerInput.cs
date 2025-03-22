using Minifarm._Core.EventService;
using Minifarm.Common.Events;
using Minifarm.Factory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minifarm.GameInput
{
    public class PlayerInput : MonoBehaviour
    {
        private Camera mainCamera;
        private BaseFactory currentFactory;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out BaseFactory factory))
                {
                    if (currentFactory != null && currentFactory != factory)
                        currentFactory.ChangeSelectedState(false);

                    factory.OnClick();
                    currentFactory = factory;
                }

            }
            else
            {
                if (currentFactory != null)
                {
                    currentFactory.ChangeSelectedState(false);
                    currentFactory = null;
                }
                GameEventService.Fire(new EmptyClickEvent { });
            }
        }
    }
}