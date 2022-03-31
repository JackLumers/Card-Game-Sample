using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CardGameSample.Scripts.Input
{
    public sealed class HoldableObjectInputHandler : MonoBehaviour
    {
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private RectTransform itemsRect;
        [SerializeField] private Camera renderingCamera;
        [SerializeField] private float itemMovingSpeed = 100f;

        public Camera Camera => renderingCamera;
        [CanBeNull] public IHoldableObject HoldenObject { get; private set; }
        public Vector2 LastPointerPosition { get; private set; }

        /// <summary>
        /// 1nd arg - moved game object
        /// 2nd arg - screen point position
        /// </summary>
        /// 
        public event Action<GameObject, Vector2> Moved;
        
        /// <summary>
        /// 1nd arg - holden game object
        /// 2nd arg - screen point position
        /// 3nd arg - is hold started or ended
        /// </summary>
        public event Action<GameObject, Vector2, bool> Holden;

        private void OnEnable()
        {
            // Touched or left button clicked
            inputModule.leftClick.action.performed += OnLeftClick;

            // Touch or pointer moved
            inputModule.point.action.performed += OnPointMoved;
        }

        private void OnDisable()
        {
            // Touched or left button clicked
            inputModule.leftClick.action.performed += OnLeftClick;

            // Touch or pointer moved
            inputModule.point.action.performed += OnPointMoved;

            if (!ReferenceEquals(HoldenObject, null))
            {
                HoldenObject.Hold(LastPointerPosition, false);
                Holden?.Invoke(HoldenObject.GameObject, LastPointerPosition, false);
                HoldenObject = null;
            }
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!CheckTouchscreenAvailable()) return;
            float value = context.ReadValue<float>();
            var touch = Touchscreen.current.primaryTouch;
            LastPointerPosition = touch.position.ReadValue();
                
            // Pointer down
            if (value > 0)
            {
                // For setting result in GetLastRaycastResult
                inputModule.Process();

                int touchId = touch.touchId.ReadValue();
                var touchedGameObject = inputModule.GetLastRaycastResult(touchId).gameObject;

                if (!ReferenceEquals(touchedGameObject, null))
                {
                    // Ignore if it's not holdable object
                    if (!touchedGameObject.TryGetComponent(out IHoldableObject holdableItem)) return;
                        
                    // Ignore if it's the same object (this action can be called 2 times by the InputSystem so we need to check it)
                    if (!ReferenceEquals(HoldenObject, null) && holdableItem == HoldenObject) return;

                    HoldenObject = holdableItem;
                    // ReSharper disable once PossibleNullReferenceException
                    HoldenObject.Hold(LastPointerPosition, true);
                    Holden?.Invoke(HoldenObject.GameObject, LastPointerPosition, true);
                }
            }
            // Pointer up
            else
            {
                if (ReferenceEquals(HoldenObject, null)) return;
                    
                HoldenObject.Hold(LastPointerPosition, false);
                Holden?.Invoke(HoldenObject.GameObject, LastPointerPosition, false);
                HoldenObject = null;
            }
        }
        
        private void OnPointMoved(InputAction.CallbackContext context)
        {
            if (!CheckTouchscreenAvailable()) return;
            if (ReferenceEquals(HoldenObject, null)) return;

            var touch = Touchscreen.current.primaryTouch;
            LastPointerPosition = touch.position.ReadValue();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemsRect, LastPointerPosition, renderingCamera,
                out Vector2 calculatedPosition);

            HoldenObject.MoveByHold(calculatedPosition, itemMovingSpeed);
            Moved?.Invoke(HoldenObject.GameObject, LastPointerPosition);
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