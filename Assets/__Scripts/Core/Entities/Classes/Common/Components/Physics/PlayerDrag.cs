using Assemblies.Input;
using Assemblies.Network.NetworkLifecycle;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Common.Components.Physics
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
            ClassStaticData classStaticData,
            InputReader inputReader,
            GroundCheck groundCheck,
            PhysicsWrapper physicsWrapper,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _inputReader = inputReader;
            _physicsWrapper = physicsWrapper;
            _groundCheck = groundCheck;
            _groundDrag = classStaticData.commonStaticData.groundDrag;
            _airDrag = classStaticData.commonStaticData.airDrag;

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
            
            float decay = _groundCheck.IsOnGroundNoCoyote ? _groundDrag : _airDrag;
            
            _physicsWrapper.SetVelocity(x : _physicsWrapper.Velocity.x * decay);
        }
    }
}
