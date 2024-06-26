﻿using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions
{
    [UsedImplicitly]
    public class PlayerJump : INetworkLifecycleOwnerObserver, IToggleableMovement
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        public bool HoldingJump { get; private set; }

        private int MaxJumps => _classNetworkDataContainer.ActionMapData.jumpCount;
        private bool FirstJump => _jumpsLeft == MaxJumps;
        private CommonStaticData StaticData => _playerStaticData.commonStaticData;

        private readonly PhysicsWrapper _physicsWrapper;
        private readonly InputReader _inputReader;
        private readonly GroundCheck _groundCheck;
        private readonly IClassNetworkDataContainer _classNetworkDataContainer;
        private readonly PlayerStaticData _playerStaticData;

        private int _jumpsLeft;
        
        public PlayerJump(
            PlayerStaticData playerStaticData,
            PhysicsWrapper physicsWrapper,
            InputReader inputReader,
            GroundCheck groundCheck,
            IClassNetworkDataContainer classNetworkDataContainer,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _playerStaticData = playerStaticData;
            _classNetworkDataContainer = classNetworkDataContainer;
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
