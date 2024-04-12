using System;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using JetBrains.Annotations;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    [UsedImplicitly]
    public class PlayerGravity : INetworkLifecycleOwnerObserver, IFixedTickable, IToggleablePhysics
    {
        public bool Disabled { get; set; }
        public bool IsOwner { get; set; }
        
        private CommonStaticData CommonStaticData => _staticData.commonStaticData;

        private readonly PlayerJump _playerJump;
        private readonly PlayerStaticData _staticData;
        private readonly PhysicsWrapper _physicsWrapper;
        private readonly GroundCheck _groundCheck;

        private IDisposable _groundCheckDisposable;

        public PlayerGravity(
            PlayerStaticData playerStaticData,
            PhysicsWrapper physicsWrapper,
            GroundCheck groundCheck,
            PlayerJump playerJump,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _playerJump = playerJump;
            _groundCheck = groundCheck;
            _physicsWrapper = physicsWrapper;
            _staticData = playerStaticData;
            
            networkLifecycleSubject.Register(this);
            actionToggler.Register(this);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            _groundCheckDisposable = _groundCheck.IsOnGround.Subscribe(OnGroundChanged);
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _groundCheckDisposable?.Dispose();
        }
        
        public void FixedTick()
        {
            if (Disabled)
                return;
            
            if (!IsOwner)
                return;

            if (_groundCheck.IsOnGround.Value)
                return;
            
            HandleAirGravity();
        }

        private void HandleAirGravity()
        {
            if (_physicsWrapper.Velocity.y > 0 && !_playerJump.HoldingJump)
                SetGravityMultiplier(CommonStaticData.jumpCutGravityMult);
            else if (Mathf.Abs(_physicsWrapper.Velocity.y) < CommonStaticData.jumpHangVelocityThreshold)
                SetGravityMultiplier(CommonStaticData.jumpHangGravityMult);
            else if (_physicsWrapper.Velocity.y < 0)
                SetGravityMultiplier(CommonStaticData.fallGravityMult);
        }

        private void OnGroundChanged(bool onGround)
        {
            if (Disabled)
                return;
            
            if (onGround)
                _physicsWrapper.SetGravityScale(CommonStaticData.GravityScale);
        }
        
        private void SetGravityMultiplier(float mult) =>
            _physicsWrapper.SetGravityScale(CommonStaticData.GravityScale * mult);
    }
}
