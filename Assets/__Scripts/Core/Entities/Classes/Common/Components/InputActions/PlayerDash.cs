using Assemblies.Input;
using Assemblies.Network.NetworkLifecycle;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using Assemblies.Utilities.Timers;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Common.Data.Player;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Enums;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    [UsedImplicitly]
    public class PlayerDash : IStartable, ITickable, INetworkLifecycleOwnerObserver, IToggleableAbility
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        private ActionMapData ActionMapData => ClassDataContainer.ActionMapData;

        protected readonly PhysicsWrapper physicsWrapper;
        protected readonly CollidersWrapper collidersWrapper;
        protected readonly IEntityModel playerModel;
        protected readonly IClassDataContainer ClassDataContainer;
        protected readonly ClassStaticData staticData;

        private readonly InputReader _inputReader;
        private readonly ActionToggler _actionToggler;

        private CountdownTimer _cdTimer;
        private float _startGravityScale;

        public PlayerDash(
            InputReader inputReader,
            IClassDataContainer classDataContainer,
            PhysicsWrapper physicsWrapper,
            CollidersWrapper collidersWrapper,
            IEntityModel playerModel,
            ClassStaticData staticData,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            this.staticData = staticData;
            this.collidersWrapper = collidersWrapper;
            this.playerModel = playerModel;
            this.physicsWrapper = physicsWrapper;
            this.ClassDataContainer = classDataContainer;

            
            
            _actionToggler = actionToggler;
            _inputReader = inputReader;

            actionToggler.Register(this);
            networkLifecycleSubject.Register(this);
        }

        public void Start()
        {
            if (!IsOwner)
                return;
            
            _cdTimer = new CountdownTimer(ActionMapData.dashCd);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Dash += OnDashInput;
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Dash -= OnDashInput;
        }

        public void Tick()
        {
            if (!IsOwner)
                return;

            _cdTimer.Tick(Time.deltaTime);
        }

        private void OnDashInput(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;

            if (!context.performed)
                return;

            if (_cdTimer.IsRunning)
                return;

            _cdTimer.Start();
            
            Dash();
        }

        protected virtual TweenerCore<Vector2, Vector2, VectorOptions> DashAction(RaycastHit2D hit, float dashSpeed)
        {
            if (hit.transform == null)
            {
                return physicsWrapper.Rigidbody2D
                    .DOMoveX(ActionMapData.dashRange * playerModel.FacingSign, dashSpeed)
                    .SetRelative()
                    .SetSpeedBased();
            }
            return physicsWrapper.Rigidbody2D
                .DOMoveX(hit.point.x - collidersWrapper.HalfHitBoxWidth * playerModel.FacingSign, dashSpeed)
                .SetSpeedBased();
        }

        protected virtual void OnBeforeDashStart()
        {
            _startGravityScale = physicsWrapper.GravityScale;
            
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
            
            //_collidersWrapper.HitBox.enabled = false;
        }

        protected virtual void OnDashComplete()
        {
            physicsWrapper.SetGravityScale(_startGravityScale);
            _actionToggler.EnableAll();
            
            //_collidersWrapper.HitBox.enabled = true;
        }

        protected virtual LayerMask DashLayerMask() =>
            staticData.commonStaticData.dashBlockLayerMask;

        private async void Dash()
        {
            float dashRange = ActionMapData.dashRange;
            float dashSpeed = staticData.commonStaticData.DashSpeed(dashRange, ClassDataContainer.Speed);

            OnBeforeDashStart();
            
            RaycastHit2D hit = Physics2D.BoxCast(
                physicsWrapper.Position,
                collidersWrapper.HitBoxSize,
                0,
                playerModel.FacingDirectionVector,
                dashRange,
                DashLayerMask());

            await DashAction(hit, dashSpeed).AsyncWaitForCompletion();
            
            OnDashComplete();
        }
    }
}
