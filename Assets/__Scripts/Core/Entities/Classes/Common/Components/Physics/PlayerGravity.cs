using Assemblies.Network.NetworkLifecycle;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Common.Components.Physics
{
    [UsedImplicitly]
    public class PlayerGravity : INetworkLifecycleOwnerObserver, IFixedTickable, IToggleablePhysics
    {
        public bool Disabled { get; set; }
        public bool IsOwner { get; set; }
        
        private CommonStaticData CommonStaticData => _staticData.commonStaticData;

        private readonly PlayerJump _playerJump;
        private readonly ClassStaticData _staticData;
        private readonly PhysicsWrapper _physicsWrapper;
        private readonly GroundCheck _groundCheck;
        
        public PlayerGravity(
            ClassStaticData classStaticData,
            PhysicsWrapper physicsWrapper,
            GroundCheck groundCheck,
            PlayerJump playerJump,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _playerJump = playerJump;
            _groundCheck = groundCheck;
            _physicsWrapper = physicsWrapper;
            _staticData = classStaticData;
            
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
                _physicsWrapper.SetGravityScale(CommonStaticData.gravityScale);
        }
        
        private void SetGravityMultiplier(float mult) =>
            _physicsWrapper.SetGravityScale(CommonStaticData.gravityScale * mult);
    }
}
