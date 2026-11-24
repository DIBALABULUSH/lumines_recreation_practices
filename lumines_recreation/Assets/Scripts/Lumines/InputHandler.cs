using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lumines
{
    public class InputHandler : MonoBehaviour, InputActions.ILuminesStandardActions
    {
        public event Action<Vector2> MovePieceAxis;
        public event Action RotateCw;
        public event Action RotateCcw;
        public event Action SoftDrop;
        public event Action HardDrop;

        public Vector2 MoveInput { get; private set; }

        private InputActions _inputActions;
        
        private void Awake()
        {
            _inputActions = new InputActions();
            _inputActions.LuminesStandard.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _inputActions.LuminesStandard.Enable();
        }

        private void OnDisable()
        {
            // Disabling the map cancels active movement and notifies subscribers.
            _inputActions.LuminesStandard.Disable();
            MoveInput = Vector2.zero;
        }

        private void OnDestroy()
        {
            _inputActions.LuminesStandard.SetCallbacks(null);
            _inputActions.Dispose();
        }

        public void OnMovePiece(InputAction.CallbackContext context)
        {
            if (!context.performed && !context.canceled)
                return;
            
            MoveInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
            MovePieceAxis?.Invoke(MoveInput);
        }

        public void OnMoveUp(InputAction.CallbackContext context)
        {
        }

        public void OnMoveDown(InputAction.CallbackContext context)
        {
        }

        public void OnMoveLeft(InputAction.CallbackContext context)
        {
        }

        public void OnMoveRight(InputAction.CallbackContext context)
        {
        }

        public void OnRotateCw(InputAction.CallbackContext context)
        {
            if (context.performed)
                RotateCw?.Invoke();
        }

        public void OnRotateCcw(InputAction.CallbackContext context)
        {
            if (context.performed)
                RotateCcw?.Invoke();
        }

        public void OnSoftDrop(InputAction.CallbackContext context)
        {
            if (context.performed)
                SoftDrop?.Invoke();
        }

        public void OnHardDrop(InputAction.CallbackContext context)
        {
            if (context.performed)
                HardDrop?.Invoke();
        }
    }
}
