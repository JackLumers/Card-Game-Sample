using ThirdPartyUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CardGameSample.Scripts.Input
{
    public class HoldableItemInputController : MonoBehaviour
    {
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private Camera renderingCamera;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField] private Canvas itemsCanvas;

        private IHoldableItem _holdenItem;
        
        private void Awake()
        {
            // Touched or left button clicked
            inputModule.leftClick.action.performed += context =>
            {
                float value = context.ReadValue<float>();
                
                // Hold
                if (value > 0)
                {
                    if (!CheckTouchscreenAvailable()) return;
                    
                    int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
                    var touchedGameObject = inputModule.GetLastRaycastResult(touchId).gameObject;

                    if (!ReferenceEquals(touchedGameObject, null))
                    {
                        if(touchedGameObject.TryGetComponent(out IHoldableItem holdableItem))
                        {
                            // Ignore if it's the same object
                            if (ReferenceEquals(_holdenItem, null) || holdableItem != _holdenItem)
                            {
                                _holdenItem = holdableItem;
                                _holdenItem.Hold(true);
                            }
                        }
                        Debug.Log($"Touched gameObject name: {touchedGameObject.name}");
                    }
                }
                // Cancel hold
                else
                {
                    Debug.Log($"Canceled touch gameObject name: {_holdenItem}");
                    _holdenItem?.Hold(false);
                    _holdenItem = null;
                }
            };

            // Touch pointer moved
            inputModule.point.action.performed += context =>
            {
                if (!CheckTouchscreenAvailable()) return;
                if (ReferenceEquals(_holdenItem, null)) return;

                var touch = Touchscreen.current.primaryTouch;
                var screenPosition = touch.position.ReadValue();
                var calculatedPosition = itemsCanvas.ScreenToCanvasPosition(screenPosition);
                
                Debug.Log($"Screen position: {screenPosition}");
                Debug.Log($"Calculated position: {calculatedPosition}");
                
                _holdenItem.Move(calculatedPosition);
            };
        }

        private bool CheckTouchscreenAvailable()
        {
            if (Touchscreen.current != null)
            {
                return true;
            }

            Debug.LogWarning("Can't get the Touchscreen. Make sure you activated the Touchscreen simulation in the Input System Debugger.");
            return false;
        }
    }
}