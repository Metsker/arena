using Assemblies.Input;
using Assemblies.Network.NetworkLifecycle;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    [UsedImplicitly]
    public class PlayerJump : INetworkLifecycleOwnerObserver, IToggleableMovement
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        public bool HoldingJump { get; private set; }

        private int MaxJumps => _classDataContainer.ActionMapData.jumpCount;
        private bool FirstJump => _jumpsLeft == MaxJumps;
        private CommonStaticData StaticData => _classStaticData.commonStaticData;

        private readonly PhysicsWrapper _physicsWrapper;
        private readonly InputReader _inputReader;
        private readonly GroundCheck _groundCheck;
        private readonly IClassDataContainer _classDataContainer;
        private readonly ClassStaticData _classStaticData;
        
        private int _jumpsLeft;
        
        public PlayerJump(
            ClassStaticData classStaticData,
            PhysicsWrapper physicsWrapper,
            InputReader inputReader,
            GroundCheck groundCheck,
            IClassDataContainer classDataContainer,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _classStaticData = classStaticData;
            _classDataContainer = classDataContainer;
            _groundCheck = groundCheck;
            _inputReader = inputReader;
            _physicsWrapper = physicsWrapper;

            networkLifecycleSubject.Register(this);
            actionToggler.Register(this);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Jump += OnJump;
            _groundCheck.isOnGround.OnValueChanged += OnGroundChanged;
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Jump -= OnJump;
            _groundCheck.isOnGround.OnValueChanged -= OnGroundChanged;
        }

        private void OnGroundChanged(bool _, bool onGround)
        {
            if (onGround)
                _jumpsLeft = MaxJumps;
            else if (_physicsWrapper.Velocity.y < 0)
                _jumpsLeft--;
        }

        private void OnJump(InputAction.CallbackContext callbackContext)
        {
            HoldingJump = !callbackContext.canceled;

            if (!CanJump(callbackContext))
                return;

            float jumpForce = FirstJump ? StaticData.jumpForce : StaticData.jumpForce * StaticData.secondaryJumpsForceModifier;

            _jumpsLeft--;

            _physicsWrapper.SetGravityScale(StaticData.gravityScale);
            _physicsWrapper.SetVelocity(y: jumpForce);
        }

        private bool CanJump(InputAction.CallbackContext callbackContext)
        {
            if (Disabled || !callbackContext.performed)
                return false;

            return _jumpsLeft > 0;
        }
    }
}
