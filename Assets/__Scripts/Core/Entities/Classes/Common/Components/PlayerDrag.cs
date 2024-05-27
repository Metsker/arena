using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using JetBrains.Annotations;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    [UsedImplicitly]
    public class PlayerDrag : IFixedTickable, INetworkOwnershipObserver, IToggleablePhysics
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }

        private readonly GroundCheck _groundCheck;
        private readonly PhysicsWrapper _physicsWrapper;
        private readonly InputReader _inputReader;
        
        private readonly float _groundDrag;
        private readonly float _airDrag;

        public PlayerDrag(
            PlayerStaticData playerStaticData,
            InputReader inputReader,
            GroundCheck groundCheck,
            PhysicsWrapper physicsWrapper,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _inputReader = inputReader;
            _physicsWrapper = physicsWrapper;
            _groundCheck = groundCheck;
            _groundDrag = playerStaticData.commonStaticData.groundDrag;
            _airDrag = playerStaticData.commonStaticData.airDrag;

            networkLifecycleSubject.Register(this);
            actionToggler.Register(this);
        }

        public void FixedTick()
        {
            if (!IsOwner)
                return;
            
            if (Disabled)
                return;
            
            if (_inputReader.MoveVector.x != 0)
                return;

            if (_physicsWrapper.Velocity.x == 0)
                return;
            
            float decay = _groundCheck.IsActuallyOnGround ? _groundDrag : _airDrag;
            
            _physicsWrapper.SetVelocity(x : _physicsWrapper.Velocity.x * decay);
        }
    }
}
