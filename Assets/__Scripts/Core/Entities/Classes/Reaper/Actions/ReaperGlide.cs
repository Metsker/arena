using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Assemblies.Input;
using Assemblies.Utilities.Extensions;
using Assemblies.Utilities.Timers;
using DG.Tweening;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.InputActions.Enums;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Reaper.Data;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Enums;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Tower.Core.Entities.Classes.Reaper.Actions
{
    public class ReaperGlide : NetworkBehaviour, IToggleableAbility
    {
        public bool Disabled { get; set; }
        private float DashSpeed(float distance) => _classStaticData.commonStaticData.DashSpeed(distance, _reaperDataContainer.Speed);

        private BackstebDash _backstabDash;
        private InputReader _inputReader;
        private ReaperDataContainer _reaperDataContainer;
        private IEntityModel _playerModel;
        private CollidersWrapper _collidersWrapper;
        private ClassStaticData _classStaticData;
        private ActionToggler _actionToggler;
        private PhysicsWrapper _physicsWrapper;

        private IHealth _targetHealth;
        private Direction _startDirection;
        private CountdownTimer _cdTimer;
        private float _startGravityScale;

        [Inject]
        private void Construct(
            ClassStaticData classStaticData,
            CollidersWrapper collidersWrapper,
            IEntityModel playerModel,
            ReaperDataContainer reaperDataContainer,
            BackstebDash backstebDash,
            InputReader inputReader,
            ActionToggler actionToggler,
            PhysicsWrapper physicsWrapper)
        {
            _physicsWrapper = physicsWrapper;
            _actionToggler = actionToggler;
            _classStaticData = classStaticData;
            _collidersWrapper = collidersWrapper;
            _playerModel = playerModel;
            _reaperDataContainer = reaperDataContainer;
            _backstabDash = backstebDash;
            _inputReader = inputReader;
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _inputReader.Action1 += OnActionInput;
                _cdTimer = new CountdownTimer(_reaperDataContainer.ActionMapData.action1Cd);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Action1 -= OnActionInput;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            _cdTimer.Tick(Time.deltaTime);
        }

        private void OnActionInput(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;

            if (_cdTimer.IsRunning)
                return;
            
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                _playerModel.FacingDirectionVector,
                _reaperDataContainer.ReaperStats.action1CastRange,
                _classStaticData.commonStaticData.attackLayerMask);

            if (hit.transform == null || !hit.transform.TryGetComponent(out _targetHealth))
            {
                Debug.LogWarning("No target");
                return;
            }
            
            _cdTimer.Start(_reaperDataContainer.ActionMapData.action1Cd);
            
            StartCoroutine(BounceGlidingCoroutine(hit));
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private IEnumerator BounceGlidingCoroutine(RaycastHit2D hit)
        {
            Vector3 startPos = transform.position;
            
            _collidersWrapper.PhysicsBox.enabled = false;
            //_collidersWrapper.HitBox.enabled = false;

            _startGravityScale = _physicsWrapper.GravityScale;
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
            
            yield return TweenToPoint(hit.transform.position, 15);
            DealDamage(0.5f);
            
            yield return new WaitForSeconds(0.3f);
            
            
            yield return TweenToPoint(OppositeBottomCorner(hit.collider), 20);
            DealDamage(0.2f);

            yield return new WaitForSeconds(0.2f);
            
            yield return TweenToPoint(AdjacentTopCorner(hit.collider), 20);
            
            DealDamage(1f);
            
            //yield return _backstabDash.TweenToPoint(hit.transform.position, 15).WaitForCompletion();
            //DealDamage(0.5f);
            
            yield return new WaitForSeconds(0.2f);
            
            yield return TweenToPoint(AdjacentBottomCorner(hit.collider), 20);
            DealDamage(0.2f);
            
            yield return TweenToPoint(OppositeTopCorner(hit.collider), 20);
            DealDamage(0.5f);
            
            yield return TweenToPoint(new Vector2(transform.position.x, startPos.y), 5, _backstabDash.ExitOffset);
            
            _collidersWrapper.PhysicsBox.enabled = true;
            //_collidersWrapper.HitBox.enabled = true;
            
            _physicsWrapper.SetGravityScale(_startGravityScale);
            _actionToggler.EnableAll();
        }

        private void DealDamage(float modifier) =>
            _targetHealth.DealDamageRpc(
                _reaperDataContainer.ReaperStats.glideBaseDamage +
                _reaperDataContainer.Damage * modifier);

        private Vector2 OppositeTopCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.max : col2D.bounds.max.With(x: col2D.bounds.min.x);

        private Vector2 AdjacentBottomCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.min : col2D.bounds.min.With(x: col2D.bounds.max.x);

        private Vector2 AdjacentTopCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.max.With(x: col2D.bounds.min.x) : col2D.bounds.max;

        private Vector2 OppositeBottomCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.min.With(x: col2D.bounds.max.x) : col2D.bounds.min;
        
        //TODO: TEST WITH FRAME-BASED TARGETING
        private YieldInstruction TweenToPoint(Vector2 point, float dashSpeed, Vector2 offset = default) =>
            _physicsWrapper.Rigidbody2D
                .DOMove(point + offset, dashSpeed)
                .SetSpeedBased()
                .WaitForCompletion();
    }
}
