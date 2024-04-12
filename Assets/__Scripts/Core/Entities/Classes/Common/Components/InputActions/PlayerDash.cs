using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Collisions;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions
{
    [UsedImplicitly]
    public class PlayerDash : IStartable, ITickable, INetworkLifecycleOwnerObserver, IToggleableAbility
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        private ActionMapData ActionMapData => classNetworkDataContainer.ActionMapData;

        protected readonly PhysicsWrapper physicsWrapper;
        protected readonly CollidersWrapper collidersWrapper;
        protected readonly IEntityModel playerModel;
        protected readonly IClassNetworkDataContainer classNetworkDataContainer;
        protected readonly PlayerStaticData staticData;

        private readonly InputReader _inputReader;
        private readonly ActionToggler _actionToggler;

        private CountdownTimer _cdTimer;
        private float _startGravityScale;

        public PlayerDash(
            InputReader inputReader,
            IClassNetworkDataContainer classNetworkDataContainer,
            PhysicsWrapper physicsWrapper,
            CollidersWrapper collidersWrapper,
            IEntityModel playerModel,
            PlayerStaticData staticData,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            this.staticData = staticData;
            this.collidersWrapper = collidersWrapper;
            this.playerModel = playerModel;
            this.physicsWrapper = physicsWrapper;
            this.classNetworkDataContainer = classNetworkDataContainer;

            
            
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
            float dashSpeed = staticData.commonStaticData.DashSpeed(dashRange, classNetworkDataContainer.Speed);

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
