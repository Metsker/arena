using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Arena._Project._Scripts
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Arena/Input Reader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        public Vector2 Move => _inputActions.Player.Movement.ReadValue<Vector2>();
        public Vector2 MousePos => _inputActions.Player.MousePosition.ReadValue<Vector2>();

        public event Action<InputAction.CallbackContext> ShotPerformed;

        private Controls _inputActions;

        private void OnEnable()
        {
            if (_inputActions != null)
                return;
            
            _inputActions = new Controls();
            _inputActions.Player.SetCallbacks(this);
        }

        public void Enable() =>
            _inputActions.Enable();

        public void Disable() =>
            _inputActions.Disable();

        public void OnMovement(InputAction.CallbackContext context)
        {
            // noop
        }
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            // noop
        }
        public void OnShooting(InputAction.CallbackContext context)
        {
            if (context.performed)
                ShotPerformed?.Invoke(context);
        }
    }
}
