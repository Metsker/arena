using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assemblies.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Arena/Input Reader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        private const float DeadZone = 0.1f;
        
        [SerializeField] private float bufferTime = 0.4f;
        
        public Vector2 MoveVector { get; private set; }
        public bool IsMovingDown => MoveVector.y < -DeadZone;
        public bool IsEnabled { get; private set; }
        public event Action<InputAction.CallbackContext> Attack;
        public event Action<InputAction.CallbackContext> Move;
        public event Action<InputAction.CallbackContext> FallThrough;
        public event Action<InputAction.CallbackContext> Jump;
        public event Action<InputAction.CallbackContext> Dash;
        public event Action<InputAction.CallbackContext> Ultimate;
        public event Action<InputAction.CallbackContext> Action1;
        public event Action<InputAction.CallbackContext> Action2;
        public event Action<InputAction.CallbackContext> Menu;

        private Controls _inputActions;

        private void OnEnable()
        {
            if (_inputActions != null)
                return;
            
            _inputActions = new Controls();
            _inputActions.Player.SetCallbacks(this);
        }

        public void Enable()
        {
            _inputActions.Enable();
            IsEnabled = true;
        }

        public void Disable()
        {
            _inputActions.Disable();
            IsEnabled = false;
        }

        public InputBuffer BuildBuffer() =>
            new (bufferTime);

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveVector = context.ReadValue<Vector2>();
            Move?.Invoke(context);
        }

        public void OnJump(InputAction.CallbackContext context) =>
            Jump?.Invoke(context);
        
        public void OnAttack(InputAction.CallbackContext context) =>
            Attack?.Invoke(context);
        
        public void OnDash(InputAction.CallbackContext context) =>
            Dash?.Invoke(context);
        
        public void OnUltimate(InputAction.CallbackContext context) =>
            Ultimate?.Invoke(context);
        
        public void OnAction1(InputAction.CallbackContext context) =>
            Action1?.Invoke(context);
        
        public void OnAction2(InputAction.CallbackContext context) =>
            Action2?.Invoke(context);
        
        public void OnMenu(InputAction.CallbackContext context) =>
            Menu?.Invoke(context);
        
        public void OnFallThrough(InputAction.CallbackContext context) =>
            FallThrough?.Invoke(context);
    }
}
