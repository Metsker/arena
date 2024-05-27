using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using JetBrains.Annotations;
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
            _groundCheck.isOnGround.OnValueChanged += OnGroundChanged;
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _groundCheck.isOnGround.OnValueChanged -= OnGroundChanged;
        }
        
        public void FixedTick()
        {
            if (Disabled)
                return;
            
            if (!IsOwner)
                return;

            if (_groundCheck.isOnGround.Value)
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

        private void OnGroundChanged(bool _, bool onGround)
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
